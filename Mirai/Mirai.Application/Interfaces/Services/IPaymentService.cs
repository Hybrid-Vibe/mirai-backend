using Mirai.Domain.Entities;
using Mirai.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Mirai.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections);
    }
}
