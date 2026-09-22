using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler(IIdentityService identityService ,
    ITokenService  tokenService) : IRequestHandler<LoginCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // get user by email
        var userResult = await identityService.GetUserByEmailAsync(request.LoginDto.Email);
        if (!userResult.IsSuccess)
            return Result<UserDto>.Fail(Error.InvalidCredentials());

        // check user password
        var passwordResult = await identityService.CheckPasswordAsync(request.LoginDto.Email, request.LoginDto.Password);
        if (!passwordResult.IsSuccess)
            return Result<UserDto>.Fail(passwordResult.Errors);

        var user = userResult.Value;
        var resultRoles = await identityService.GetRolesAsync(user.Email , cancellationToken);
        var roles = resultRoles.Value;
        
        // refresh token
        var refreshTokenResult  = tokenService.GenerateRefreshToken();
        await identityService.SaveRefreshTokenAsync(user.Id, refreshTokenResult , cancellationToken);
        
        return Result<UserDto>.Ok(new UserDto
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = tokenService.CreateToken(user.Id , user.Email , user.UserName , roles),
            RefreshToken = refreshTokenResult.Token,
            RefreshTokenExpiration = refreshTokenResult.ExpiresOn
            
        });
    }
}
