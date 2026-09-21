using Project = Domain.Entities.Project;

namespace Application.Contracts;

public interface IProjectRepository
{
    // Create project
    public Task CreateAsync(Project project, CancellationToken cancellationToken);

    // Get all projects
    public Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken);
    
    // get by id 
    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken);

    // update
    Task UpdateAsync(Project project);

    // soft delete
    Task DeleteAsync(Project project);

    // used for uniqueness validation
    Task<bool> HasNameAsync(string name);
}
