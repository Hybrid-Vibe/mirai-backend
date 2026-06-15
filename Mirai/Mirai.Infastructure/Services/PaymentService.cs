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
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mirai.Infastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
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

        public async Task<string> CreatePayOSUrl(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Order not found");


            if (string.IsNullOrEmpty(_configuration["PayOS:ReturnUrl"]))
                throw new Exception("ReturnUrl missing");

            if (string.IsNullOrEmpty(_configuration["PayOS:CancelUrl"]))
                throw new Exception("CancelUrl missing");

            // Extract the variables to format the signature string correctly
            var amount = Convert.ToInt32(order.TotalAmount);
            var description = order.OrderNumber.Substring(0, Math.Min(25, order.OrderNumber.Length));
            var returnUrl = _configuration["PayOS:ReturnUrl"];
            var cancelUrl = _configuration["PayOS:CancelUrl"];
            var orderCode = order.PayosOrderCode;

            // Generate the signature
            var signatureData = $"amount={amount}&cancelUrl={cancelUrl}&description={description}&orderCode={orderCode}&returnUrl={returnUrl}";
            var checksumKey = _configuration["PayOS:ChecksumKey"];

            using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(checksumKey));
            var hashBytes = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signatureData));
            var signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            var requestBody = new
            {
                orderCode = orderCode,
                amount = amount,
                description = description,
                returnUrl = returnUrl,
                cancelUrl = cancelUrl,
                signature = signature // Add the required signature field
            };

            var clientId = _configuration["PayOS:ClientId"];
            var apiKey = _configuration["PayOS:ApiKey"];

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await _httpClient.PostAsJsonAsync(
                "https://api-merchant.payos.vn/v2/payment-requests",
                requestBody
            );

            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine(result);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"PayOS error: {result}");
            }

            using var json = JsonDocument.Parse(result);
            var root = json.RootElement;

            if (!root.TryGetProperty("data", out var data) ||
                data.ValueKind == JsonValueKind.Null)
            {
                throw new Exception($"Invalid PayOS response: {result}");
            }

            if (!data.TryGetProperty("checkoutUrl", out var checkoutUrlElement))
            {
                throw new Exception($"Missing checkoutUrl: {result}");
            }

            return checkoutUrlElement.GetString();

        }


        public async Task HandlePayOSWebhook(PayOSWebhookRootDto dto)
        {
            Console.WriteLine("WEBHOOK HIT");

            Console.WriteLine(
                JsonSerializer.Serialize(dto,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }));

            // ⚠️ 1. VERIFY SIGNATURE TRƯỚC TIÊN (QUAN TRỌNG)
            var isValidSignature = VerifyPayOSWebhookSignature(dto, _configuration["PayOS:ChecksumKey"]);

            if (!isValidSignature)
            {
                Console.WriteLine("INVALID PAYOS SIGNATURE - REJECT WEBHOOK");
                return; // chặn toàn bộ request giả mạo
            }

            // ⚠️ 2. CHECK ORDER
            var order = await _unitOfWork.OrderRepository
                .GetByPayOSOrderCode(dto.Data.OrderCode);

            if (order == null)
                return;

            // ⚠️ 3. CHECK PAYOS RESPONSE STATUS
            if (dto.Code != "00" || dto.Success != true)
            {
                await _unitOfWork.OrderRepository.UpdatePaymentStatus(order.OrderId, PaymentStatus.Failed);
                return;
            }

            // ⚠️ 4. CHECK DUPLICATE PAYMENT
            var existing = await _unitOfWork.PaymentRepository
                .GetByTransactionIdAsync(dto.Data.OrderCode.ToString());

            if (existing == null)
            {
                await _unitOfWork.PaymentRepository.CreatePaymentByPayOS(new PaymentDto
                {
                    OrderId = order.OrderId,
                    Amount = dto.Data.Amount,
                    TransactionId = dto.Data.OrderCode.ToString()
                });
            }

            // ⚠️ 5. UPDATE ORDER STATUS
            await _unitOfWork.OrderRepository.UpdatePaymentStatus(order.OrderId, PaymentStatus.Paid);
            await _unitOfWork.OrderRepository.UpdateOrderStatus(order.OrderId, OrderStatus.Confirmed);

            // ⚠️ 6. UPDATE PAYMENT STATUS
            await _unitOfWork.PaymentRepository.UpdatePaymentStatus(
                order.OrderId,
                PaymentStatusInPayment.Succeed
            );
        }

        private bool VerifyPayOSWebhookSignature(
    PayOSWebhookRootDto dto,
    string checksumKey)
        {
            if (dto?.Data == null ||
                string.IsNullOrWhiteSpace(dto.Signature))
            {
                return false;
            }

            var data = new Dictionary<string, object>
            {
                ["orderCode"] = dto.Data.OrderCode,
                ["amount"] = dto.Data.Amount,
                ["description"] = dto.Data.Description,
                ["accountNumber"] = dto.Data.AccountNumber,
                ["reference"] = dto.Data.Reference,
                ["transactionDateTime"] = dto.Data.TransactionDateTime,
                ["currency"] = dto.Data.Currency,
                ["paymentLinkId"] = dto.Data.PaymentLinkId,
                ["code"] = dto.Data.Code,
                ["desc"] = dto.Data.Desc,
                ["counterAccountBankId"] = dto.Data.CounterAccountBankId,
                ["counterAccountBankName"] = dto.Data.CounterAccountBankName,
                ["counterAccountName"] = dto.Data.CounterAccountName,
                ["counterAccountNumber"] = dto.Data.CounterAccountNumber,
                ["virtualAccountName"] = dto.Data.VirtualAccountName,
                ["virtualAccountNumber"] = dto.Data.VirtualAccountNumber
            };

            var sorted = data
       .Where(x => x.Value != null)
       .OrderBy(x => x.Key);
            var rawData = string.Join("&",
        sorted.Select(x => $"{x.Key}={x.Value}"));

            Console.WriteLine($"RAW DATA: {rawData}");

            var generatedSignature =
                GenerateSignature(data, checksumKey);
            Console.WriteLine($"PAYOS SIGNATURE: {dto.Signature}");
            Console.WriteLine($"LOCAL SIGNATURE: {generatedSignature}");
            return string.Equals(
                generatedSignature,
                dto.Signature,
                StringComparison.OrdinalIgnoreCase);
        }

        private string GenerateSignature(
    Dictionary<string, object> data,
    string checksumKey)
        {
            var sorted = data
                .Where(x => x.Value != null)
                .OrderBy(x => x.Key, StringComparer.Ordinal);

            var rawData = string.Join("&",
                sorted.Select(x => $"{x.Key}={x.Value}"));

            using var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(checksumKey));

            var hash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(rawData));

            return Convert.ToHexString(hash).ToLower();
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

        public async Task<UpdatePaymentStatusResponse> UpdatePaymentStatus(string orderId, PaymentStatusInPayment newStatus)
        {
            return await _unitOfWork.PaymentRepository.UpdatePaymentStatus(orderId, newStatus);
        }
    }
}
