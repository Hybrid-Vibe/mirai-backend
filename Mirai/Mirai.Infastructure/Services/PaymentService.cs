using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using SportsBicycleStore.Libraries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<Payment> CreatePaymentByCOD(PaymentByCODDto paymentByCODDto)
        {
            return await _unitOfWork.PaymentRepository.CreatePaymentByCOD(paymentByCODDto);
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(model.OrderId);
            if (order == null)
                throw new Exception("Order not found");


            var pay = new VnPayLibrary();

            var amount = Convert.ToInt64(decimal.Round((decimal)model.Amount * 100m, 0, MidpointRounding.AwayFromZero));


            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", amount.ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Amount}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", order.OrderId);
            pay.AddRequestData("vnp_ExpireDate", timeNow.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            string paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public async Task<Payment> GetByIdAsync(string id)
        {
            return await _unitOfWork.PaymentRepository.GetByIdAsync(id);
        }

        public async Task<PaymentResponseModel> PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var vnpay = new VnPayLibrary();

                foreach (var (key, value) in collections)
                {
                    if (!string.IsNullOrEmpty(key)
                        && key.StartsWith("vnp_")
                        && key != "vnp_SecureHash"
                        && key != "vnp_SecureHashType")
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                var vnp_SecureHash = collections["vnp_SecureHash"];

                bool checkSignature = vnpay.ValidateSignature(
                    vnp_SecureHash,
                    _configuration["Vnpay:HashSecret"]
                );

                if (!checkSignature)
                {
                    return new PaymentResponseModel
                    {
                        Success = false,
                        Message = "Invalid signature"
                    };
                }

                var vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

                var orderId = vnpay.GetResponseData("vnp_TxnRef");

                var paymentDto = new PaymentDto
                {
                    OrderId = orderId,
                    Amount = vnp_Amount,
                    TransactionId = vnp_TransactionId,
                    
                };
                if (vnp_ResponseCode == "00")
                {

                    // Save payment
                    await _unitOfWork.PaymentRepository.CreatePaymentByVNPay(paymentDto);

                    // Update order status
                    await _unitOfWork.OrderRepository.UpdateOrderStatus(
                        orderId,
                        OrderStatus.Confirmed
                    );

                    await _unitOfWork.OrderRepository.UpdatePaymentStatus(
                        orderId,
                        PaymentStatus.Paid
                    );

                    await _unitOfWork.PaymentRepository.UpdatePaymentStatus(orderId, PaymentStatusInPayment.Succeed);

                    return new PaymentResponseModel
                    {
                        Success = vnp_ResponseCode == "00",
                        PaymentMethod = "VnPay",
                        Amount = (decimal)vnp_Amount,
                        TransactionId = vnp_TransactionId,
                        VnPayResponseCode = vnp_ResponseCode,
                        Message = "Payment successful"
                    };
                }

                await _unitOfWork.PaymentRepository.CreatePaymentByVNPay(paymentDto);

                await _unitOfWork.OrderRepository.UpdatePaymentStatus(
                    orderId,
                    PaymentStatus.Failed
                );

                await _unitOfWork.PaymentRepository.UpdatePaymentStatus(orderId, PaymentStatusInPayment.Failed);

                return new PaymentResponseModel
                {
                    Success = false,
                    PaymentMethod = "VnPay",
                    Amount = (decimal)vnp_Amount,
                    TransactionId = vnp_TransactionId,
                    VnPayResponseCode = vnp_ResponseCode,
                    Message = "Payment failed"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponseModel { Success = false, Message = ex.Message };
            }
        }

        public async Task UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus)
        {
            await _unitOfWork.PaymentRepository.UpdatePaymentStatus(orderId, newStatus);
        }
    }
}
