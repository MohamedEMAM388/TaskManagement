namespace Application.Contracts;

public interface IUnitOfWork
{
    public IProjectRepository ProjectRepository { get; }
    public ITaskRepository TaskRepository { get; }
    public ICommentRepository CommentRepository { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
