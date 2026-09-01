using Application.Interfaces;
using MediatR;
using Task = Domain.Entities.Task;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandHandler(IUnitOfWork unitOfWork) :
                                     IRequestHandler<CreateTaskCommand , int>
{
    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = new Task()
        {
          Title = request.Title,
          Description = request.Description,
          DueDate = request.DueDate,

        };
         await  unitOfWork.TaskRepository.CreateTaskAsync(task, cancellationToken);
         await  unitOfWork.SaveChangesAsync(cancellationToken);
         return task.Id;
    }
}