using Domain.Entities;
using AsyncTask = System.Threading.Tasks.Task;

namespace Application.Interfaces;

public interface IProjectRepository
{
    
    // create project take Command => return bool
    public AsyncTask CreateAsync(Project project, CancellationToken cancellationToken);

    // get projects take query => return list of projectsDTO
    public Task<IEnumerable<Project>>  GetAllAsync(CancellationToken cancellationToken);
    
}