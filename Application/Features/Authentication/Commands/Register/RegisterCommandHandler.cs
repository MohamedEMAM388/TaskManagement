using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands.Register;

public class RegisterCommandHandler(IIdentityService identityService,
    ITokenService tokenService) : IRequestHandler<RegisterCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // check if email already exists
        var existingUserResult = await identityService.GetUserByEmailAsync(request.RegisterDto.Email);
        if (existingUserResult.IsSuccess)
            return Result<UserDto>.Fail(Error.Conflict(
                "User.EmailAlreadyExists",
                $"An account with email '{request.RegisterDto.Email}' already exists."));

        var userResult = await identityService
            .CreateUserAsync(request.RegisterDto, cancellationToken);
        if (!userResult.IsSuccess)
            return Result<UserDto>.Fail(userResult.Errors);

        var user = userResult.Value;
        var resultRoles = await identityService.GetRolesAsync(user.Email, cancellationToken);
        var roles = resultRoles.Value;

        // refresh token 
        var refreshTokenResult = tokenService.GenerateRefreshToken();
        await identityService.SaveRefreshTokenAsync(user.Id, refreshTokenResult, cancellationToken);
        return Result<UserDto>.Ok(new UserDto()
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = tokenService.CreateToken(user.Id, user.Email, user.UserName, roles),
            RefreshToken = refreshTokenResult.Token,
            RefreshTokenExpiration = refreshTokenResult.ExpiresOn
        });
    }
}