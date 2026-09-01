using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Project = Domain.Entities.Project;
using Task = System.Threading.Tasks.Task;


namespace Infrastructure.Repositories;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public async Task CreateAsync(Project project, CancellationToken cancellationToken)
    {

        await context.Projects.AddAsync(project , cancellationToken);
        
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken)
    {
        var projects = await context.Projects.ToListAsync(cancellationToken);
        return projects;
    }

    public async Task<bool> HasNameAsync(string name)
    {
        return await context.Projects
                    .AnyAsync(x => x.Name == name);
    }
}