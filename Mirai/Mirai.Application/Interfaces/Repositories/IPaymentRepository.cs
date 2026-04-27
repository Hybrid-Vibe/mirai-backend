using Mirai.Domain.Entities;
using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentByCOD(PaymentDto paymentDto);
        Task<Payment> CreatePaymentByVNPay(PaymentDto paymentDto);
    }
}
