using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class GenericRepository<TEntity , TKey> : IGenericRepository<TEntity , TKey> where TEntity : BaseEntity<TKey>
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

}