using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Authentication.Commands.Register;
using Infrastructure.Persistence.Identity;
using Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IdentityServices;

public class IdentityService(UserManager<ApplicationUser> userManager ,
    IdentityAppDbContext identityAppDbContext) : IIdentityService
{
    public async Task<Result<IdentityUserResult>> GetUserByEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<IdentityUserResult>.Fail(
                Error.NotFound("User.NotFound", $"User with email '{email}' was not found."));

        return Result<IdentityUserResult>.Ok(new IdentityUserResult(
            user.Id, email,
            user.FullName,
            user.UserName ?? string.Empty));
    }

    public async Task<Result> CheckPasswordAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Result.Fail(Error.InvalidCredentials());

        var isPasswordValid = await userManager.CheckPasswordAsync(user, password);
        return isPasswordValid
            ? Result.Ok()
            : Result.Fail(Error.InvalidCredentials());
    }

    public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct)
    {
        var user = new ApplicationUser()
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            PhoneNumber = registerDto.PhoneNumber,
        };

        var isCreated = await userManager.CreateAsync(user, registerDto.Password);
        if (isCreated.Succeeded)
            return Result<IdentityUserResult>.Ok(
                new IdentityUserResult(user.Id, user.Email, user.FullName, user.UserName));
        
        var errors = isCreated.Errors
            .Select(error => new Error(error.Code, error.Description)).ToList();
        return Result<IdentityUserResult>.Fail(errors);


    }

    public async Task<Result<IReadOnlyList<string>>> GetRolesAsync(string email, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(email);
        if(user is null)
            return Result<IReadOnlyList<string>>.Fail(new Error(
                "User.NotFound",
                $"User with this email {email} was not found",
                ErrorType.NotFound
            ));

        var roles = await userManager.GetRolesAsync(user);
        return Result<IReadOnlyList<string>>.Ok(roles.ToList());

    }

    public async Task<Result> SaveRefreshTokenAsync(string userId, RefreshTokenResult refreshToken, CancellationToken ct)
    {
        var entity = new RefreshToken()
        {
            Token = refreshToken.Token,
            ExpiresOn = refreshToken.ExpiresOn,
            CreatedOn = refreshToken.CreatedOn,
            UserId = userId
        };
        
        await identityAppDbContext.RefreshTokens.AddAsync(entity , ct);
        await identityAppDbContext.SaveChangesAsync(ct);
        return Result.Ok();
    }

    public async Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var storedToken = await identityAppDbContext.RefreshTokens.FirstOrDefaultAsync(
            x => x.Token == refreshToken, ct);
        
        if (storedToken is null)
            return Result.Fail(Error.NotFound(
                "RefreshToken.NotFound",
                "Refresh token not found"
            ));

        if (!storedToken.IsActive)
            return Result.Fail(Error.Validation(
                "RefreshToken.AlreadyRevokedOrExpired",
                "Refresh token is already revoked or expired"
            ));

        storedToken.RevokedOn = DateTime.UtcNow;
        await identityAppDbContext.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
