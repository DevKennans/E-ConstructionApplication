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
    }
}
