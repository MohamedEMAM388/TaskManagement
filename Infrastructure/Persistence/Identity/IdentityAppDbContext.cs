using Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Identity;

public class IdentityAppDbContext : IdentityDbContext<ApplicationUser>
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
    {
        
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .ToTable("Users");
        builder.Entity<ApplicationUser>()
            .Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Entity<IdentityRole>()
            .ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>()
            .ToTable("UserRoles");
    }
    
    
    
    
}