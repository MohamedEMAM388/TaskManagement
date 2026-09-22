namespace Application.Features.Authentication.Commands.Register;

public class RegisterDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}