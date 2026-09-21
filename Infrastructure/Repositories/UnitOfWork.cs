using Application.Contracts;
using Infrastructure.Persistence;
namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    public IProjectRepository ProjectRepository { get; }
    public ITaskRepository TaskRepository { get; }
    public ICommentRepository CommentRepository { get; }

    public UnitOfWork(
        AppDbContext dbContext,
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        ICommentRepository commentRepository)
    {
        _dbContext = dbContext;
        TaskRepository = taskRepository;
        ProjectRepository = projectRepository;
        CommentRepository = commentRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
