using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrder(OrderRequestDto orderRequestDto);
        Task<UpdateOrderStatusResponse> UpdateOrderStatus(string orderId, OrderStatus newStatus);
        Task<UpdateOrderStatusResponse> UpdatePaymentStatus(string orderId, PaymentStatus newStatus);
        Task<Order> GetByIdAsync(string id);
        Task<List<Order>> GetByUserIdAsync(string userId);
    }
}
