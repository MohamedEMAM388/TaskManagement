using Application.Common.Identity;
using Application.Common.ResultPattern;

namespace Application.Contracts;

public interface IIdentityService
{
    // get user by email (NotFound error if the user doesn't exist)
    public Task<Result<IdentityUserResult>> GetUserByEmailAsync(string email);

    // check user password (InvalidCredentials error if the email or password is wrong)
    public Task<Result> CheckPasswordAsync(string email, string password);
}
