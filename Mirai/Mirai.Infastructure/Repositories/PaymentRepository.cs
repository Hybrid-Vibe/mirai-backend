using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
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

        public async Task<Payment> CreatePaymentByCOD(PaymentDto paymentDto)
        {
            var payment = new Payment()
            {
                PaymentId = Guid.NewGuid().ToString(),
                OrderId = paymentDto.OrderId,
                Method = "COD",
                Provider = "COD",
                Status = "Pending",
                Amount = paymentDto.Amount,
                TransactionId = paymentDto.TransactionId,
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
                Status = "Pending",
                Amount = paymentDto.Amount,
                TransactionId = paymentDto.TransactionId,
                CreatedAt = DateTime.Now,

            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
