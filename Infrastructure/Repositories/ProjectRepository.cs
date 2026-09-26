using Application.Contracts;
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

    public async Task<IEnumerable<Project>> GetAllAsync(string? ownerId,CancellationToken cancellationToken)
    {
        var query = context.Projects.AsQueryable();
        if(ownerId is not null)
            query = query.Where(x => x.CreatedByUserId == ownerId);
        
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdWithTasksAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Projects.Include(x => x.Tasks)
            .ThenInclude(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }


    public async Task<bool> HasNameAsync(string name)
    {
        return await context.Projects
                    .AnyAsync(x => x.Name == name);
    }

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Projects.FirstOrDefaultAsync(
                             x => x.Id == id, cancellationToken);

    }

    public Task UpdateAsync(Project project)
    {
        context.Projects.Update(project);
        return Task.CompletedTask;
    }
    
}