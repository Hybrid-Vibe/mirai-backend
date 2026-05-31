using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<OrderResponseDto> CreateOrder(OrderRequestDto orderRequestDto);
        Task UpdateOrderStatus(string orderId, OrderStatus newStatus);
        Task UpdatePaymentStatus(string orderId, PaymentStatus newStatus);
        Task MarkPaymentFailed(string orderId);
        Task MarkAsPaid(string orderId);
        Task<Order> GetByIdAsync(string id);
        Task<Order> GetByUserIdAsync(string userId);
    }
}
