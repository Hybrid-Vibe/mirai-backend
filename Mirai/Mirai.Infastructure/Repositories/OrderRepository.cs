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
            var strategy = _context.Database.CreateExecutionStrategy();
            OrderResponseDto orderResponse = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
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
                        Status = (int)OrderStatus.Created,
                        PaymentStatus = (int)PaymentStatus.Unpaid,

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

                    orderResponse = new OrderResponseDto
                    {
                        OrderId = order.OrderId,
                        OrderNumber = order.OrderNumber,
                        TotalAmount = order.TotalAmount,
                        Status = (int)(OrderStatus)order.Status,
                        PaymentStatus = (int)(PaymentStatus)order.PaymentStatus,
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
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return orderResponse;
        }

        public async Task UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new Exception("Order not found");

            // FIX 1: parse enum đúng
            var currentStatus = (OrderStatus)order.Status;

            if (!OrderStateValidator.CanUpdateOrderStatus(currentStatus, newStatus))
                throw new Exception($"Invalid order status transition: {order.Status} -> {newStatus}");

            // FIX 2: check payment status đúng kiểu string
            if (newStatus == OrderStatus.Cancelled &&
                order.PaymentStatus == (int)PaymentStatus.Paid)
            {
                throw new Exception("Cannot cancel paid order without refund");
            }

            order.Status = (int)newStatus;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatus(string orderId, PaymentStatus newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new Exception("Order not found");

            // FIX: parse đúng enum
            var currentStatus = (PaymentStatus)order.PaymentStatus;

            // validate transition
            if (!OrderStateValidator.CanUpdatePaymentStatus(currentStatus, newStatus))
                throw new Exception($"Invalid payment transition: {order.PaymentStatus} -> {newStatus}");

            // update
            order.PaymentStatus = (int)newStatus;

            await _context.SaveChangesAsync();
        }

        public async Task CancelAndRefund(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != (int)PaymentStatus.Paid)
                throw new Exception("Only paid orders can be refunded");

            if (order.Status == (int)OrderStatus.Delivered)
                throw new Exception("Cannot cancel delivered order");

            using var transaction = await _context.Database.BeginTransactionAsync();

            // TODO: gọi payment gateway refund ở đây

            order.Status = (int)OrderStatus.Cancelled;
            order.PaymentStatus = (int)PaymentStatus.Refunded;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        //payment success
        public async Task MarkAsPaid(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != (int)PaymentStatus.Unpaid)
                throw new Exception("Invalid payment state");

            order.PaymentStatus = (int)PaymentStatus.Paid;

            await _context.SaveChangesAsync();
        }

        //payment failed
        public async Task MarkPaymentFailed(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId)
                ?? throw new Exception("Order not found");

            if (order.PaymentStatus != (int)PaymentStatus.Unpaid)
                throw new Exception("Invalid payment state");

            order.PaymentStatus = (int)PaymentStatus.Failed;

            // Optional: auto cancel luôn
            order.Status = (int)OrderStatus.Cancelled;

            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<Order> GetByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Shippings)
                .FirstOrDefaultAsync(o => o.UserId == userId);
        }
    }
}
