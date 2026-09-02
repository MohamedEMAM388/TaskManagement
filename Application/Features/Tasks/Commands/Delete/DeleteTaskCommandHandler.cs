using Application.Interfaces;
using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Tasks.Commands.Delete;

public class DeleteTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTaskCommand , bool>
{
    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // 1 => Task exists
        var task = await unitOfWork.TaskRepository
                   .GetTaskByIdAsync(request.Id, cancellationToken);

        if (task is null)
            return false;
        
        // 2 => Project exists
        var project = await unitOfWork.ProjectRepository
                     .GetByIdAsync(task.ProjectId, cancellationToken);

        if (project is null)
            return false;
        
        // 3 => Cannot delete task from completed or archived project
        if (project.Status is ProjectStatus.Completed or ProjectStatus.Archived)
            return false;

        await unitOfWork.TaskRepository.DeleteTaskAsync(task);
        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;


    }
}