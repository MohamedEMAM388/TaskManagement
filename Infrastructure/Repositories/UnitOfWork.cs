using System.Diagnostics.Contracts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    
    
    private readonly AppDbContext _dbContext;
    public IProjectRepository ProjectRepository { get; }

    public UnitOfWork(AppDbContext dbContext , IProjectRepository projectRepository)
    {
        _dbContext = dbContext;
        ProjectRepository = projectRepository;
    }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}