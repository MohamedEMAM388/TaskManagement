using Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Commands.update;

public class UpdateTaskCommandHandler(IUnitOfWork unitOfWork ,
    IMapper mapper) : IRequestHandler<UpdateTaskCommand , bool>
{
    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        // task exist
        var task = await unitOfWork.
                   TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return false;
        
        // project with request.project id exist
        var project = await unitOfWork.ProjectRepository
                      .GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return false;
        
        // Prevent reopening a completed task
        if (task.IsCompleted && !request.IsCompleted)
            return false;

        // update task
           mapper.Map(request, task); 
        
        await  unitOfWork.TaskRepository.UpdateTaskAsync(task);
        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
     
        
    }
}