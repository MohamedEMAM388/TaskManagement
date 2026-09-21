using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Tasks.Commands.Update;

public class UpdateTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateTaskCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Fail(TaskErrors.NotFound(request.Id));

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.TaskRepository.UpdateTaskAsync(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
