using Application.Features.Authentication.Commands.DTOs;
using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.LogOut;
using Application.Features.Authentication.Commands.RefreshToken;
using Application.Features.Authentication.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Authentication(ISender sender) : ApiBaseController
    {
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(dto);
            var result = await sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(dto);
            var result = await sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutDto request, CancellationToken cancellationToken)
        {
            var command = new LogOutCommand(request.RefreshToken);
            var result = await sender.Send(command, cancellationToken);
            return ToActionResult(result);
        }
        
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshTokenDto request,
            CancellationToken cancellationToken)
        {
            
            var result = await sender.Send(
                new RefreshTokenCommand(request.RefreshToken), cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }
    }
}