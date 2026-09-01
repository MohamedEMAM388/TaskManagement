using System.Diagnostics.Contracts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    
    
    private readonly AppDbContext _dbContext;
    public IProjectRepository ProjectRepository { get; }
    public ITaskRepository TaskRepository { get; }

    public UnitOfWork(AppDbContext dbContext , IProjectRepository projectRepository ,
        ITaskRepository taskRepository)
    {
        _dbContext = dbContext;
        TaskRepository = taskRepository;
        ProjectRepository = projectRepository;
    }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}