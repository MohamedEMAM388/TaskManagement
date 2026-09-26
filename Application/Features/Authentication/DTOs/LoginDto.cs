using System.ComponentModel.DataAnnotations;

namespace Application.Features.Authentication.Commands.DTOs;

public class LoginDto
{
    [Required , EmailAddress]
    public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}