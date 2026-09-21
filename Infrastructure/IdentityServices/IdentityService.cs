using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.IdentityServices;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
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
}
