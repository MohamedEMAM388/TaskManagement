using Domain.Entities;
using Task = Domain.Entities.Task;

namespace Application.Interfaces;

public interface IUnitOfWork
{

    public IProjectRepository ProjectRepository { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


}