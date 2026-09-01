using Application.Interfaces;
using Domain.Entities.Enums;
using MediatR;
using Task = Domain.Entities.Task;


namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandHandler(IUnitOfWork unitOfWork) :
                                     IRequestHandler<CreateTaskCommand , int>
{
    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
                                           // Business Rules //
        // 1 => project of request.projectId must be available 
         var project = await unitOfWork.ProjectRepository
                                     .GetByIdAsync(request.ProjectId, cancellationToken);

         if (project is null)
                 throw new KeyNotFoundException($"Project with id {request.ProjectId} does not exist");
         
         // 2 => if project is completed and archived can not add task
         if (project.Status is ProjectStatus.Completed or ProjectStatus.Archived)
             throw new InvalidOperationException(
                       "Cannot add task to a completed or archived project");
         
         var task = new Task()
        {
          Title = request.Title,
          Description = request.Description,
          DueDate = request.DueDate,
          ProjectId =  request.ProjectId,
        };
         await  unitOfWork.TaskRepository.CreateTaskAsync(task, cancellationToken);
         await  unitOfWork.SaveChangesAsync(cancellationToken);
         return task.Id;
    }
}