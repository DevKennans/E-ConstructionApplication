using EConstructionApp.Application.Features.Commands.Auth.SignUp;
using EConstructionApp.Application.Interfaces.Services.Identification;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Identification;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EConstructionApp.Persistence.Concretes.Services.Entities.Identification
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public AuthService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
    }
}
