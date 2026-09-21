using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // get user by email
        var userResult = await identityService.GetUserByEmailAsync(request.LoginDto.Email);
        if (!userResult.IsSuccess)
            // don't reveal whether the email exists: same error as a wrong password
            return Result<UserDto>.Fail(Error.InvalidCredentials());

        // check user password
        var passwordResult = await identityService.CheckPasswordAsync(request.LoginDto.Email, request.LoginDto.Password);
        if (!passwordResult.IsSuccess)
            return Result<UserDto>.Fail(passwordResult.Errors);

        var user = userResult.Value;
        return Result<UserDto>.Ok(new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = "Token"
        });
    }
}
