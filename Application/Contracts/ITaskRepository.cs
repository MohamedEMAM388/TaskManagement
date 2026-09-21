using DomainTask = Domain.Entities.Task;
namespace Application.Contracts;

public interface ITaskRepository
{
    // create
    public Task CreateTaskAsync(DomainTask task, CancellationToken cancellationToken);

    // update
    public Task UpdateTaskAsync(DomainTask task);

    // delete (soft delete)
    public Task DeleteTaskAsync(DomainTask task);

    // get by id
    public Task<DomainTask?> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken);

    // get all
    public Task<IEnumerable<DomainTask>> GetAllAsync(CancellationToken cancellationToken);
}
