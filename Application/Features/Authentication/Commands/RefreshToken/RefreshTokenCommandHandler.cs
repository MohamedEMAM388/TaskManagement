using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.DTOs;
using MediatR;

namespace Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(IIdentityService identityService ,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand , Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // validate on token
        var validateRefreshToken = await identityService
            .ValidateRefreshToken(request.RefreshToken, cancellationToken);

        if (!validateRefreshToken.IsValid || validateRefreshToken.UserId is null)
            return Result<UserDto>.Fail(Error.Unauthorized(
                "Auth.InvalidRefreshToken",
                validateRefreshToken.ErrorMessage ?? "Invalid or expired refresh token"));
        
        var userId = validateRefreshToken.UserId;
        if (userId is null)
            return Result<UserDto>.Fail(new Error
                ("InvalidToken", "Invalid or expired refresh token"));
        
        var userResult = await identityService.GetUserByIdAsync(userId);
        if (!userResult.IsSuccess)
            return Result<UserDto>.Fail(Error.Unauthorized(
                "Auth.InvalidRefreshToken", "Invalid or expired refresh token"));

        var user = userResult.Value;
        
        // get roles
        var rolesResult = await identityService.GetRolesAsync(user.Email, cancellationToken);
        if (!rolesResult.IsSuccess)
            return Result<UserDto>.Fail(rolesResult.Errors);
        
        // revoke token 
        await identityService.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
        
        // create access + refresh token 
        var token = tokenService.CreateToken(
            user.Id, user.Email, user.UserName, rolesResult.Value);
        
        var newRefreshToken = tokenService.GenerateRefreshToken();
        // save refresh token
        await identityService.SaveRefreshTokenAsync(userId,newRefreshToken, cancellationToken);

        return Result<UserDto>.Ok( new UserDto()
            {
             Email   =  user.Email,
             DisplayName =  user.DisplayName,
             Token =  token,
             RefreshToken = newRefreshToken.Token,
             RefreshTokenExpiration = newRefreshToken.ExpiresOn
            });


    }
}