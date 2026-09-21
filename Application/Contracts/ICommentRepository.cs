using Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Application.Contracts;

public interface ICommentRepository
{
    Task AddAsync(Comment comment, CancellationToken cancellationToken);

    Task<Comment?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken);

    Task DeleteAsync(Comment comment);
}
