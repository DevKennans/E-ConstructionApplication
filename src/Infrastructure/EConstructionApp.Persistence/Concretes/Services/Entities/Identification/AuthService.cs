﻿using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Identification;
using EConstructionApp.Application.Features.Commands.Auth.LogIn;
using EConstructionApp.Application.Features.Commands.Auth.SignUp;
using EConstructionApp.Application.Interfaces.Services.Identification;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EConstructionApp.Persistence.Concretes.Services.Entities.Identification
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenHandler _tokenHandler;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ITokenHandler tokenHandler, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenHandler = tokenHandler;
            _unitOfWork = unitOfWork;
        }

        public async Task<SignUpCommandResponse> SignUpAsync(SignUpCommandRequest? signUpCommandRequest)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>()
                .GetAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: e => e.PhoneNumber == signUpCommandRequest!.PhoneNumber);
            if (employee is null)
                return new SignUpCommandResponse
                {
                    IsSuccess = false,
                    Message = "We couldn't find an active employee with this phone number. Please check the number or contact your admin."
                };
            if (employee.IsDeleted)
                return new SignUpCommandResponse
                {
                    IsSuccess = false,
                    Message = "The employee associated with this phone number is no longer active. Please contact your administrator for assistance."
                };

            bool userExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == signUpCommandRequest!.PhoneNumber);
            if (userExists)
                return new SignUpCommandResponse
                {
                    IsSuccess = false,
                    Message = "This phone number is already registered. Please use a different number or login if you're an existing user."
                };

            AppUser newUser = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                UserName = signUpCommandRequest!.PhoneNumber,
                NormalizedUserName = signUpCommandRequest.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = signUpCommandRequest.PhoneNumber,
                PhoneNumberConfirmed = false
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, signUpCommandRequest.Password);
            if (!result.Succeeded)
                return new SignUpCommandResponse
                {
                    IsSuccess = false,
                    Message = string.Join(" ", result.Errors.Select(e => e.Description))
                };

            AppRole? employeeRole = await _roleManager.FindByNameAsync("Employee");
            IdentityResult roleResult = await _userManager.AddToRoleAsync(newUser, employeeRole!.Name!);
            if (!roleResult.Succeeded)
                return new SignUpCommandResponse
                {
                    IsSuccess = false,
                    Message = string.Join(" ", roleResult.Errors.Select(e => e.Description))
                };

            return new SignUpCommandResponse
            {
                IsSuccess = true,
                Message = "Sign-up successful. Welcome to the team!"
            };
        }

        public Task<IList<string>> GetUserAllRoles(AppUser user)
        {
            return _userManager.GetRolesAsync(user);
        }

        public async Task<LogInCommandResponse> LogInAsync(LogInCommandRequest? logInCommandRequest)
        {
            bool isPhoneNumber = Regex.IsMatch(logInCommandRequest!.UsernameOrPhoneNumber, @"^\d{10,15}$");

            AppUser? user;
            if (isPhoneNumber)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == logInCommandRequest.UsernameOrPhoneNumber);
                if (user is null)
                    return new LogInCommandResponse { IsSuccess = false, Message = "Invalid phone number." };
            }
            else
            {
                user = await _userManager.FindByNameAsync(logInCommandRequest.UsernameOrPhoneNumber);
                if (user == null)
                    return new LogInCommandResponse { IsSuccess = false, Message = "Invalid username." };
            }

            if (!await _userManager.CheckPasswordAsync(user, logInCommandRequest.Password))
                return new LogInCommandResponse { IsSuccess = false, Message = "Incorrect password." };

            if (!string.IsNullOrWhiteSpace(logInCommandRequest.DeviceToken))
            {
                user.DeviceToken = logInCommandRequest.DeviceToken;
                await _userManager.UpdateAsync(user);
            }

            IList<string> roles = await GetUserAllRoles(user);

            Token token = _tokenHandler.CreateAccessToken(seconds: 1 * 60 * 60, user, roles);
            await UpdateRefreshToken(token.RefreshToken, user, token.ExpirationDate, addOnAccessTokenDate: 1 * 15 * 60);

            return new LogInCommandResponse
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token
            };
        }

        public async Task<Token?> RefreshTokenAsync(string? refreshToken)
        {
            if (refreshToken is null)
                return null!;

            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user is not null && user.RefreshTokenEndDate > DateTime.UtcNow)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user!);

                Token token = _tokenHandler.CreateAccessToken(seconds: 1 * 60 * 60, user, roles);
                await UpdateRefreshToken(token.RefreshToken, user, token.ExpirationDate, addOnAccessTokenDate: 1 * 15 * 60);
                return token;
            }
            else return null!;
        }

        public async System.Threading.Tasks.Task UpdateRefreshToken(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);

            await _userManager.UpdateAsync(user);
        }

        public async Task<IList<string>> GetFcmTokensByEmployeeIdsAsync(List<Guid> employeeIds)
        {
            var employees = await _unitOfWork.GetReadRepository<Employee>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: e => employeeIds.Contains(e.Id)
                );

            var employeePhoneNumbers = employees
                .Select(e => e.PhoneNumber)
                .ToList();

            if (!employeePhoneNumbers.Any())
                return new List<string>();

            var tokens = await _userManager.Users
                .Where(u => u.PhoneNumber != null &&
                            employeePhoneNumbers.Contains(u.PhoneNumber) &&
                            !string.IsNullOrEmpty(u.DeviceToken))
                .Select(u => u.DeviceToken!)
                .ToListAsync();

            return tokens;
        }

        public async Task<(bool IsSuccess, string Message)> LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            user.DeviceToken = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            return (true, "Logout successful. Device token removed.");
        }
    }
}
