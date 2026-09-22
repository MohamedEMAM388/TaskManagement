using Application.Common.Identity;

namespace Application.Contracts;

public interface ITokenService
{
    // create token => return string 
    public string CreateToken(string userId , string email, string userName , IReadOnlyList<string> roles);
    
    // refresh token
    RefreshTokenResult GenerateRefreshToken();
}