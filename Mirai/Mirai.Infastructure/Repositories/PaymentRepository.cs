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
                Method = (int)PaymentMethod.COD,
                Provider = (int)PaymentMethod.COD,
                Status = (int)PaymentStatusInPayment.Pending,
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
                Method = (int)PaymentMethod.VNPay,
                Provider = (int)PaymentMethod.VNPay,
                Status = (int)PaymentStatusInPayment.Pending,
                Amount = paymentDto.Amount,
                TransactionId = paymentDto.TransactionId,
                CreatedAt = DateTime.Now,

            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdatePaymentStatusByPaymentId(string paymentId, PaymentStatusInPayment newStatus)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId)
                ?? throw new Exception("Payment not found");

            if (!Enum.IsDefined(typeof(PaymentStatusInPayment), payment.Status))
            {
                throw new Exception($"Invalid payment status in DB: {payment.Status}");
            }
            var currentStatus = (PaymentStatusInPayment)payment.Status; 
            if (!PaymentStateValidator.CanUpdatePaymentStatus(currentStatus, newStatus))
                throw new Exception($"Invalid payment status transition: {payment.Status} -> {newStatus}");

            payment.Status = (int)newStatus;
            payment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId)
                ?? throw new Exception("Payment not found");

            if (!Enum.IsDefined(typeof(PaymentStatusInPayment), payment.Status))
            {
                throw new Exception($"Invalid payment status in DB: {payment.Status}");
            }
            var currentStatus = (PaymentStatusInPayment)payment.Status; 
            if(!PaymentStateValidator.CanUpdatePaymentStatus(currentStatus, newStatus))
                throw new Exception($"Invalid payment status transition: {payment.Status} -> {newStatus}");
            
            payment.Status = (int)newStatus;
            payment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<Payment> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }


    }
}
