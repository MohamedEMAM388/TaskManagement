using System.Diagnostics.Contracts;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    
    private readonly Dictionary<Type , object> _repositories = [];
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
    {
        var repotype = typeof(TEntity);
        if(_repositories.TryGetValue(repotype, out var repo))
            return (IGenericRepository<TEntity, TKey>)repo; // casting
        
        var newRepo = new GenericRepository<TEntity, TKey>(_dbContext);
        _repositories.Add(repotype, newRepo);
        return newRepo;
    }

    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}