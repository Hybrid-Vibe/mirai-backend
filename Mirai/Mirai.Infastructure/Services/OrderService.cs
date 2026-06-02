using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResponseDto> CreateOrder(OrderRequestDto orderRequestDto)
        {
            return await _unitOfWork.OrderRepository.CreateOrder(orderRequestDto);
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await _unitOfWork.OrderRepository.GetByIdAsync(id);
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await _unitOfWork.OrderRepository.GetByUserIdAsync(userId);
        }

        public async Task<UpdateOrderStatusResponse> UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            return await _unitOfWork.OrderRepository.UpdateOrderStatus(orderId, newStatus);
        }

        public async Task<UpdateOrderStatusResponse> UpdatePaymentStatus(string orderId, PaymentStatus newStatus)
        {
            return await _unitOfWork.OrderRepository.UpdatePaymentStatus(orderId, newStatus);
        }
    }
}
