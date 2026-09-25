using System.Security.Claims;
using Application.Common.Identity;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.IdentityServices;

public class UserService: IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?
                            .FindFirstValue(ClaimTypes.NameIdentifier);

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
}