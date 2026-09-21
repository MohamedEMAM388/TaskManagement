using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName {get; set; } = null!;
}