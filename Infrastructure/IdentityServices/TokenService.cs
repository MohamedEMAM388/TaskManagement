using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Identity;
using Application.Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.IdentityServices;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string CreateToken(string userId, string email, string userName, IReadOnlyList<string> roles)
    {
      
        // claims 
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, userName),
        };
        
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        
        // signinCredentials [secret key + algo]
        var secretKey = _jwtSettings.SecretKey;
        if (string.IsNullOrEmpty(secretKey))
              throw new InvalidOperationException("Secret key not set");
        
        if(secretKey.Length < 32)
             throw  new InvalidOperationException("Secret key is too short.");
        
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var signinCredentials = new SigningCredentials(
                                key, SecurityAlgorithms.HmacSha256);
        
        // Audience + Issuer => from jwt setting 
        var token = new JwtSecurityToken(
                  issuer: _jwtSettings.Issuer,
                  audience: _jwtSettings.Audience,
                  claims: claims,
                  expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                  signingCredentials: signinCredentials
            );

       return   new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshTokenResult GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);

        return new RefreshTokenResult()
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow
        };
    }
}

public class JwtSettings
{
    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpirationMinutes { get; init; }  
}