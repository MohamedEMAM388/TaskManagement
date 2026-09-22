using API.Extensions;
using Application.Features.Authentication.Commands.DTOs;
using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.LogOut;
using Application.Features.Authentication.Commands.Register;
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
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        
        // register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(dto);
            var result = await sender.Send(command, cancellationToken);
            
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        
        // logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutDto request, CancellationToken cancellationToken)
        {
            var command = new LogOutCommand(request.RefreshToken);
            var result = await sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem(); 
        }
    }
}
