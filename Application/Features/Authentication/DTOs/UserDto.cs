namespace Application.Features.Authentication.Commands.DTOs;

public class UserDto
{
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Token { get; set; } = null!;
    
    // refresh token
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiration { get; set; }

}