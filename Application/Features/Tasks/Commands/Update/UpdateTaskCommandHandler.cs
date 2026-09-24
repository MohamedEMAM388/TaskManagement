using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Tasks.Commands.Update;

public class UpdateTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService) : IRequestHandler<UpdateTaskCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Fail(Error.NotFound(
                "Task.NotFound",
                $"Task with ID {request.Id} was not found"));

        var userId = userService.UserId;
        if (userId is null)
            return Result.Fail(Error.Validation(
                "User.NotAuthenticated", "User is not authenticated"));

        if (task.CreatedByUserId != userId)
            return Result.Fail(Error.Forbidden(
                "Task.Forbidden", "You are not allowed to modify this task."));
        
        
        if (task.IsClosed)
            return Result.Fail(Error.Conflict("Task.AlreadyClosed",
                $"Cannot modify a task that is {task.Status}."));

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.TaskRepository.UpdateTaskAsync(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}