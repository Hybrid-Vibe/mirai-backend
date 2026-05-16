using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.ValidateOrderStatus;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Data;
using Mirai.Infastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<OrderResponseDto> CreateOrder(OrderRequestDto orderRequestDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal? subtotal = 0;
                var orderId = Guid.NewGuid().ToString();
                var order = new Order
                {
                    OrderId = orderId,
                    UserId = orderRequestDto.UserId,
                    OrderNumber = $"ORD-{DateTime.Now.Ticks}",
                    Currency = "VNĐ",
                    Status = (OrderStatus.Created).ToString(),
                    PaymentStatus = (PaymentStatus.Unpaid).ToString(),

                    Note = orderRequestDto.Note,
                    CreatedAt = DateTime.Now,
                    PlacedAt = DateTime.Now
                };
                var orderItems = new List<OrderItem>();

                var variantIds = orderRequestDto.Products.Select(p => p.VariantId).ToList();

                var variants = await _context.ProductVariants
                    .Include(v => v.Product)
                    .Where(v => variantIds.Contains(v.VariantId))
                    .ToDictionaryAsync(v => v.VariantId);

                foreach (var item in orderRequestDto.Products)
                {
                    if (!variants.TryGetValue(item.VariantId, out var variant))
                        throw new Exception($"Variant {item.VariantId} not found");


                    if (item.Quantity <= 0)
                        throw new Exception("Quantity must be > 0");

                    var lineTotal = variant.Price * item.Quantity;

                    subtotal += lineTotal;
                    orderItems.Add(new OrderItem
                    {
                        OrderItemId = Guid.NewGuid().ToString(),
                        OrderId = orderId,
                        VariantId = variant.VariantId,
                        Quantity = item.Quantity,
                        ProductName = variant.Product.Name,
                        VariantName = $"{variant.Color} - {variant.PhoneModel}",
                        Price = lineTotal,
                        UnitPrice = variant.Price,
                        DiscountAmount = 0

                    });
                }
                order.Subtotal = subtotal;
                order.DiscountAmount = 0;
                order.ShippingFee = 0;
                order.TaxAmount = 0;
                order.TotalAmount = (decimal)(subtotal ?? 0);

                await _context.Orders.AddAsync(order);
                await _context.OrderItems.AddRangeAsync(orderItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    PaymentStatus = order.PaymentStatus,
                    CreatedAt = order.CreatedAt,

                    Items = orderItems.Select(x => new OrderItemResponseDto
                    {
                        ProductName = x.ProductName,
                        VariantName = x.VariantName,
                        Quantity = x.Quantity,
                        UnitPrice = (decimal)x.UnitPrice,
                        Price = (decimal)x.Price,

                    }).ToList()
                };

                }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new Exception("Order not found");

            // FIX 1: parse enum đúng
            var currentStatus = Enum.Parse<OrderStatus>(order.Status);

            if (!OrderStateValidator.CanUpdateOrderStatus(currentStatus, newStatus))
                throw new Exception($"Invalid order status transition: {order.Status} -> {newStatus}");

            // FIX 2: check payment status đúng kiểu string
            if (newStatus == OrderStatus.Cancelled &&
                order.PaymentStatus == PaymentStatus.Paid.ToString())
            {
                throw new Exception("Cannot cancel paid order without refund");
            }

            order.Status = newStatus.ToString();

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatus(string orderId, PaymentStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new Exception("Order not found");

            // FIX: parse đúng enum
            if (!Enum.TryParse(order.PaymentStatus, out PaymentStatus currentStatus))
            {
                throw new Exception($"Invalid payment status in DB: {order.PaymentStatus}");
            }

            // validate transition
            if (!OrderStateValidator.CanUpdatePaymentStatus(currentStatus, newStatus))
                throw new Exception($"Invalid payment transition: {order.PaymentStatus} -> {newStatus}");

            // update
            order.PaymentStatus = newStatus.ToString();

            await _context.SaveChangesAsync();
        }

        public async Task CancelAndRefund(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != PaymentStatus.Paid.ToString())
                throw new Exception("Only paid orders can be refunded");

            if (order.Status == OrderStatus.Delivered.ToString())
                throw new Exception("Cannot cancel delivered order");

            using var transaction = await _context.Database.BeginTransactionAsync();

            // TODO: gọi payment gateway refund ở đây

            order.Status = OrderStatus.Cancelled.ToString();
            order.PaymentStatus = PaymentStatus.Refunded.ToString();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        //payment success
        public async Task MarkAsPaid(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != PaymentStatus.Unpaid.ToString())
                throw new Exception("Invalid payment state");

            order.PaymentStatus = PaymentStatus.Paid.ToString();

            await _context.SaveChangesAsync();
        }

        //payment failed
        public async Task MarkPaymentFailed(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != PaymentStatus.Unpaid.ToString())
                throw new Exception("Invalid payment state");

            order.PaymentStatus = PaymentStatus.Failed.ToString();

            // Optional: auto cancel luôn
            order.Status = OrderStatus.Cancelled.ToString();

            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }
    }
}
