using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.ValidateOrderStatus;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment> CreatePaymentByCOD(PaymentByCODDto paymentByCODDto)
        {
            var payment = new Payment()
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = paymentByCODDto.OrderId,
                Method = "COD",
                Provider = "COD",
                Status = (PaymentStatusInPayment.Pending).ToString(),
                Amount = paymentByCODDto.Amount,
                TransactionId = "0",
                CreatedAt = DateTime.Now,

            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> CreatePaymentByVNPay(PaymentDto paymentDto)
        {
            var payment = new Payment()
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = paymentDto.OrderId,
                Method = "VNPay",
                Provider = "VNPay",
                Status = (PaymentStatusInPayment.Pending).ToString(),
                Amount = paymentDto.Amount,
                TransactionId = paymentDto.TransactionId,
                CreatedAt = DateTime.Now,

            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId)
                ?? throw new Exception("Payment not found");

            if (!Enum.TryParse(payment.Status, out PaymentStatusInPayment currentStatus))
            {
                throw new Exception($"Invalid payment status in DB: {payment.Status}");
            }
            if(!PaymentStateValidator.CanUpdatePaymentStatus(currentStatus, newStatus))
                throw new Exception($"Invalid payment status transition: {payment.Status} -> {newStatus}");
            
            payment.Status = newStatus.ToString();
            payment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<Payment> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }


    }
}
