using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace Mirai.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> RegisterUserAsync(RegisterUserDto dto);
        Task<User?> GetByEmailAsync(string email);
        Task<List<GetUserDto>> GetAllUsersAsync();
        Task<GetUserDto?> GetUserByIdAsync(string userId);
        Task SyncSupabaseUserAsync(SyncSupabaseUserDto dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto dto);
        Task<User?> GetByIdAsync(string id);
        Task<User?> UpdateProfileUserAsync(string userId, UpdateProfileUserDto dto);
        Task<User?> UpdateProfileUserForAdminAsync(string userId, UpdateProfileUserByAdminDto dto);
        Task<User?> UpdateRefreshTokenForUser(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

    }
}
