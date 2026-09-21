using API.Extensions;
using Application.Features.Authentication.Commands.DTOs;
using Application.Features.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication(ISender sender) : ControllerBase
    {
        
        // login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(dto);
            var result = await sender.Send(command, cancellationToken);
            if (!result.IsSuccess)
                return result.ToProblem();

            return Ok(result.Value);
        }
    }
}
