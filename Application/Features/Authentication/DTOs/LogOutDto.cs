using Application.Common.Identity;
namespace Application.Features.Authentication.Commands.DTOs;

public class LogOutDto
{
    public string RefreshToken { get; set; } = string.Empty;
}