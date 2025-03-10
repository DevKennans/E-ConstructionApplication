using EConstructionApp.Application.Features.Commands.Auth.LogIn;
using EConstructionApp.Application.Features.Commands.Auth.SignUp;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommandRequest signUpCommandRequest)
        {
            SignUpCommandResponse signUpCommandResponse = await _mediator.Send(signUpCommandRequest);
            if (!signUpCommandResponse.IsSuccess)
                return BadRequest(new { signUpCommandResponse.IsSuccess, signUpCommandResponse.Message });

            return Ok(new { signUpCommandResponse.IsSuccess, signUpCommandResponse.Message });
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogInCommandRequest logInCommandRequest)
        {
            LogInCommandResponse logInCommandResponse = await _mediator.Send(logInCommandRequest);
            if (!logInCommandResponse.IsSuccess)
                return BadRequest(new { logInCommandResponse.IsSuccess, logInCommandResponse.Message });

            return Ok(new { logInCommandResponse.IsSuccess, logInCommandResponse.Message, logInCommandResponse.Token });
        }
    }
}
