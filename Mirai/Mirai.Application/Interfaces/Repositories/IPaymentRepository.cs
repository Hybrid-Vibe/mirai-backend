using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentByCOD(PaymentByCODDto paymentByCODDto);
        Task<Payment> CreatePaymentByVNPay(PaymentDto paymentDto);
        Task<Payment> CreatePaymentByPayOS(PaymentDto paymentDto);
        Task<UpdatePaymentStatusResponse> UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus);
        Task UpdatePaymentStatusByPaymentId(string paymentId, PaymentStatusInPayment newStatus);
        Task<Payment> GetByIdAsync(string id);
        Task<Payment> GetByTransactionIdAsync(string transactionId);
    }
}
