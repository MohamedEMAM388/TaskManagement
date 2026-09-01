using Project = Domain.Entities.Project;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IProjectRepository
{
    // Create project
    public Task CreateAsync(Project project, CancellationToken cancellationToken);

    // Get all projects
    public Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken);

    // Check if project with the same name exists
    public Task<bool> HasNameAsync(string name);
}