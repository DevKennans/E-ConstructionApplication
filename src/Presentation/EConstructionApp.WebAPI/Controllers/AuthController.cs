using EConstructionApp.Application.Features.Commands.Auth.LogIn;
using EConstructionApp.Application.Features.Commands.Auth.RefreshToken;
using EConstructionApp.Application.Features.Commands.Auth.SignUp;
using EConstructionApp.Application.Interfaces.Services.Identification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;
        public AuthController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;
            _authService = authService;
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommandRequest signUpCommandRequest)
        {
            SignUpCommandResponse signUpCommandResponse = await _mediator.Send(signUpCommandRequest);
            if (!signUpCommandResponse.IsSuccess)
                return BadRequest(new { signUpCommandResponse.IsSuccess, signUpCommandResponse.Message });

            return Ok(new { signUpCommandResponse.IsSuccess, signUpCommandResponse.Message });
        }

        [HttpPost("LogIn")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] LogInCommandRequest logInCommandRequest)
        {
            LogInCommandResponse logInCommandResponse = await _mediator.Send(logInCommandRequest);
            if (!logInCommandResponse.IsSuccess)
                return BadRequest(new { logInCommandResponse.IsSuccess, logInCommandResponse.Message });

            return Ok(new { logInCommandResponse.IsSuccess, logInCommandResponse.Message, logInCommandResponse.Token });
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommandRequest refreshTokenCommandRequest)
        {
            RefreshTokenCommandResponse refreshTokenCommandResponse = await _mediator.Send(refreshTokenCommandRequest);
            if (refreshTokenCommandResponse.Token is null)
                return BadRequest();
            else
                return Ok(new { refreshTokenCommandResponse.Token });
        }

        [HttpPost("LogOut")]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut([FromQuery] string userId)
        {
            await _authService.LogoutAsync(userId);
            return Ok();
        }
    }
}
