using Mirai.Application.DTO;
using Mirai.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterUserDto request);
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
        Task<List<GetUserDto>> GetAllUsersAsync();
        Task<GetUserDto?> GetUserByIdAsync(string userId);
        Task SyncSupabaseUserAsync(SyncSupabaseUserDto dto);
    }
}
