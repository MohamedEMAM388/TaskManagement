using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Features.Authentication.Commands.Register;

namespace Application.Contracts;

public interface IIdentityService
{
    // get user by email (NotFound error if the user doesn't exist)
    public Task<Result<IdentityUserResult>> GetUserByEmailAsync(string email);

    // check user password (InvalidCredentials error if the email or password is wrong)
    public Task<Result> CheckPasswordAsync(string email, string password);
    
    // create user 
    public Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto,CancellationToken ct);
    
    // get roles
    public Task<Result<IReadOnlyList<string>>> GetRolesAsync(string email, CancellationToken ct);
    
    // save refresh token for a user
    public Task<Result> SaveRefreshTokenAsync(string userId, RefreshTokenResult refreshToken, CancellationToken ct);
    
    Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
