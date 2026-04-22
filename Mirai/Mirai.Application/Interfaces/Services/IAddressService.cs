using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IAddressService
    {
        Task<Address?> CreateAddressAsync(AddressDto addressDto);
        Task<Address?> UpdateAddressAsync(UpdateAddressDto updateAddressDto, string addressId);
        Task<List<Address>> GetAddressByUserIdAsync(string userId);
        Task<Address?> GetAddressByIdAsync(string addressId);
        Task<List<Address>?> GetAllAddressesAsync();
    }
}
