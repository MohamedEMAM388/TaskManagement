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
            return Result.Fail(TaskErrors.NotFound(request.Id));

        var userId = userService.UserId;
        if (userId is null)
            return Result.Fail(Error.Validation(
                "User.NotAuthenticated", "User is not authenticated"));

        if (task.CreatedByUserId != userId)
            return Result.Fail(Error.Forbidden(
                "Task.Forbidden", "You are not allowed to modify this task."));

        task.Title = request.Title;
        task.Description = request.Description;
        task.DueDate = request.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.TaskRepository.UpdateTaskAsync(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}