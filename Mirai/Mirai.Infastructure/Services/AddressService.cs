using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class AddressService : IAddressService
    {
        public readonly IUnitOfWork _unitOfWork;
        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Address?> CreateAddressAsync(AddressDto addressDto)
        {
            return await _unitOfWork.AddressRepository.CreateAddressAsync(addressDto);
        }

        public async Task<Address?> GetAddressByIdAsync(string addressId)
        {
            return await _unitOfWork.AddressRepository.GetAddressByIdAsync(addressId);
        }

        public async Task<List<Address>> GetAddressByUserIdAsync(string userId)
        {
            return await _unitOfWork.AddressRepository.GetAddressByUserIdAsync(userId);
        }

        public async Task<List<Address>?> GetAllAddressesAsync()
        {
            return await _unitOfWork.AddressRepository.GetAllAddressesAsync();
        }

        public async Task<Address?> UpdateAddressAsync(UpdateAddressDto updateAddressDto, string addressId)
        {
            return await _unitOfWork.AddressRepository.UpdateAddressAsync(updateAddressDto, addressId);
        }
    }
}
