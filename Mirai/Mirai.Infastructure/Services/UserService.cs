using Microsoft.EntityFrameworkCore;
using Mirai.Application.DTO;
using Mirai.Application.Interfaces.Repositories;
using Mirai.Application.Interfaces.Services;
using Mirai.Domain.Entities;
using Mirai.Infastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mirai.Infastructure.Services
{
    public class UserService : IUserService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenGenerator jwtTokenGenerator;
        public UserService(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<List<GetUserDto>> GetAllUsersAsync()
        {
            return await _unitOfWork.UserRepository.GetAllUsersAsync();
        }

        public Task<GetUserDto?> GetUserByIdAsync(string userId)
        {
            return _unitOfWork.UserRepository.GetUserByIdAsync(userId);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash); // Use BCrypt.Net.BCrypt
            if (!isValid)
            {
                return null;
            }
            var token = jwtTokenGenerator.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.RoleId
            };
        }

        public async Task<bool> RegisterAsync(RegisterUserDto request)
        {
            var exitingEmail = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email);
            if (exitingEmail != null)
            {
                return false;
            }
            await _unitOfWork.UserRepository.RegisterUserAsync(request);
            return true;
        }

        
    }
}
