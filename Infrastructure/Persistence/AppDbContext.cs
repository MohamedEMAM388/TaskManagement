using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = Domain.Entities.Task;


namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Comment> Comments { get; set; }
    

}