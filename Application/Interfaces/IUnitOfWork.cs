using Domain.Entities;
using Task = Domain.Entities.Task;

namespace Application.Interfaces;

public interface IUnitOfWork
{

    public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    public Task<int> SaveChangesAsync();


}