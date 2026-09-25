using System.Security.Claims;

namespace Application.Common.Identity;

public interface IUserService
{
    string? UserId { get; }
    
    ClaimsPrincipal? User { get; }  
}