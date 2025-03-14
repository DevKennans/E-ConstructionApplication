using EConstructionApp.Application.Features.Commands.AppUser.UpdatePassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            UpdatePasswordCommandResponse updatePasswordCommandResponse = await _mediator.Send(updatePasswordCommandRequest);
            if (!updatePasswordCommandResponse.IsSuccess)
                return BadRequest(updatePasswordCommandResponse);

            return Ok(updatePasswordCommandResponse);
        }
    }
}
