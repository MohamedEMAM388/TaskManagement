using Application.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class CommentRepository(AppDbContext context) : ICommentRepository
{
    public async Task AddAsync(Comment comment, CancellationToken cancellationToken)
    {
        await context.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task<Comment?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.Comments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Comment>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken)
    {
        return await context.Comments
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task DeleteAsync(Comment comment)
    {
        comment.IsDeleted = true;
        comment.DeletedAt = DateTime.UtcNow;

        context.Comments.Update(comment);
        return Task.CompletedTask;
    }
}
