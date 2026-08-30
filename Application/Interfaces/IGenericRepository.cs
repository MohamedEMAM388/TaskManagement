using Domain.Entities;

namespace Application.Interfaces;

public interface IGenericRepository<TEntity , TKey> where TEntity : BaseEntity<TKey> 
{
    
}