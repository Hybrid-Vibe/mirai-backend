using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Address?> CreateAddressAsync(AddressDto addressDto)
        {
            var address = new Address()
            {
                AddressId = Guid.NewGuid().ToString(),
                UserId = addressDto.UserId,
                RecipientName = addressDto.RecipientName,
                RecipientPhone = addressDto.RecipientPhone,
                AddressLine = addressDto.AddressLine,
                Ward = addressDto.Ward,
                District = addressDto.District,
                City = addressDto.City,
                Province = addressDto.Province,
                Note = addressDto.Note,
                IsDefault = false,
                CreatedAt = DateTime.Now
            };
            await _context.AddAsync(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> GetAddressByIdAsync(string addressId)
        {
            return await _context.Addresses.Where(a => a.AddressId == addressId).FirstOrDefaultAsync();
        }

        public async Task<List<Address>> GetAddressByUserIdAsync(string userId)
        {
            return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<List<Address>?> GetAllAddressesAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address?> UpdateAddressAsync(UpdateAddressDto updateAddressDto, string addressId)
        {
            var address = _context.Addresses.FirstOrDefault(a => a.AddressId == addressId);
            if (address == null)
            {
                return null;
            }
            address.AddressId = addressId;
            address.RecipientName = updateAddressDto.RecipientName;
            address.RecipientPhone = updateAddressDto.RecipientPhone;
            address.AddressLine = updateAddressDto.AddressLine;
            address.Ward = updateAddressDto.Ward;
            address.District = updateAddressDto.District;
            address.City = updateAddressDto.City;
            address.Province = updateAddressDto.Province;
            address.Note = updateAddressDto.Note;
            address.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return address;
        }
    }
}
