using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AsyncTask = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public async AsyncTask CreateAsync(Project project, CancellationToken cancellationToken)
    {

        await context.Projects.AddAsync(project , cancellationToken);
        
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken)
    {
        var projects = await context.Projects.ToListAsync(cancellationToken);
        return projects;
    }
}