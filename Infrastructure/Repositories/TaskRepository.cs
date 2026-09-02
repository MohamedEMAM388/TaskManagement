using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DomainTask = Domain.Entities.Task;
using AsyncTask = System.Threading.Tasks.Task;

namespace Infrastructure.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async AsyncTask CreateTaskAsync(
        DomainTask task,
        CancellationToken cancellationToken)
    {
        await context.AddAsync(task, cancellationToken);
    }

    public AsyncTask UpdateTaskAsync(DomainTask task)
    {
        context.Update(task);

        return AsyncTask.CompletedTask;
    }

    public AsyncTask DeleteTaskAsync(DomainTask task)
    {
        context.Remove(task);

        return AsyncTask.CompletedTask;
    }

    public async Task<DomainTask?> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken)
    {
        return await context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);
    }
}