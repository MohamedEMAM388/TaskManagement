using DomainTask = Domain.Entities.Task;

namespace Application.Interfaces;

public interface ITaskRepository
{
    // create
    public Task CreateTaskAsync(DomainTask task, CancellationToken cancellationToken);

    // update
    public Task UpdateTaskAsync(DomainTask task);

    // delete
    public Task DeleteTaskAsync(DomainTask task); 
    

}