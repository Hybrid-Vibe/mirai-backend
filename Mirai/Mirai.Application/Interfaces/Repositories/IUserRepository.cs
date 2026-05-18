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
    }
}
