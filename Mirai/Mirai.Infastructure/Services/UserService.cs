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

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            bool isValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user?.PasswordHash); 
            if (!isValid) {
                return false;
            }
             await _unitOfWork.UserRepository.ChangePasswordAsync(userId, request);
            return true;
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


            var accessToken = jwtTokenGenerator.GenerateAccessToken(user);

            var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.Now.AddDays(7);

            await _unitOfWork.UserRepository.UpdateRefreshTokenForUser(user);

            
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,    
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.RoleId
            };
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

            if (user == null)
                return;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _unitOfWork.UserRepository.UpdateRefreshTokenForUser(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.UserRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
                throw new UnauthorizedAccessException();

            if (user.RefreshTokenExpiryTime <DateTime.Now)
            {
                throw new UnauthorizedAccessException();
            }

            var newAccessToken =
                jwtTokenGenerator.GenerateAccessToken(user);

            var newRefreshToken =
                jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiryTime =
                DateTime.Now.AddDays(7);

            await _unitOfWork.UserRepository.UpdateRefreshTokenForUser(user);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
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

        public async Task SyncSupabaseUserAsync(SyncSupabaseUserDto dto)
        {
            await _unitOfWork.UserRepository.SyncSupabaseUserAsync(dto);
        }

        public async Task<bool> UpdateProfileUserAsync(string userId, UpdateProfileUserDto dto)
        {
            await _unitOfWork.UserRepository.UpdateProfileUserAsync(userId, dto);
            return true;
        }

        public async Task<bool> UpdateProfileUserForAdminAsync(string userId, UpdateProfileUserByAdminDto dto)
        {
            await _unitOfWork.UserRepository.UpdateProfileUserForAdminAsync(userId, dto);
            return true;
        }
    }
}
