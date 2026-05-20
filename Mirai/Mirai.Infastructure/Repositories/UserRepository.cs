using MediatR;
using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Domain.Entities;
using Mirai.Domain.Enum;
using Mirai.Infastructure.Data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Role = Mirai.Domain.Enum.Role;

namespace Mirai.Infastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                return false;
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<List<GetUserDto>> GetAllUsersAsync()
        {
            return _context.Users
                .Select(u => new GetUserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<GetUserDto?> GetUserByIdAsync(string userId)
        {
            return _context.Users.Where(u => u.UserId == userId)
                .Select(u => new GetUserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> RegisterUserAsync(RegisterUserDto dto)
        {
            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash),
                Phone = dto.Phone,
                RoleId = ((int)Role.Customer).ToString(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task SyncSupabaseUserAsync(SyncSupabaseUserDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == dto.SupabaseUid);

            if (existingUser == null)
            {
                var user = new User
                {
                    UserId = dto.SupabaseUid,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    RoleId = "3",
                    IsActive = true,
                    AvatarUrl = dto.AvatarUrl,
                    CreatedAt = DateTime.Now
                };

                await _context.Users.AddAsync(user);

                var account = new Account
                {
                    AccountId = Guid.NewGuid().ToString(),
                    UserId = dto.SupabaseUid,
                    Provider = "google",
                    ProviderAccountId = dto.SupabaseUid,
                    CreatedAt = DateTime.Now
                };

                await _context.Accounts.AddAsync(account);

                await _context.SaveChangesAsync();
            }
            else
            {
                existingUser.FullName = dto.FullName;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> UpdateProfileUserAsync(string userId, UpdateProfileUserDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.AvatarUrl = dto.AvatarUrl;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateProfileUserForAdminAsync(string userId, UpdateProfileUserByAdminDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return null;
            }

            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.AvatarUrl = dto.AvatarUrl;
            user.RoleId = dto.RoleId;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return user;
        }
    }
}
