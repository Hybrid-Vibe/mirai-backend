using Microsoft.AspNetCore.Http;
using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
        Task<UpdatePaymentStatusResponse> UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus);
        Task<Payment> CreatePaymentByCOD(PaymentByCODDto paymentByCODDto);
        Task<Payment> GetByIdAsync(string id);
        Task<string> CreatePayOSUrl(string orderId);
        Task HandlePayOSWebhook(PayOSWebhookRootDto dto);
    }
}
