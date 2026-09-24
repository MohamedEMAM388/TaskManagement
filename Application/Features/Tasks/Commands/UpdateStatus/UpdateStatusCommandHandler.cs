using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateStatus;

public class UpdateStatusCommandHandler(IUnitOfWork unitOfWork, IUserService userService)
    : IRequestHandler<UpdateStatusCommand, Result<TaskStatus>>
{
    public async Task<Result<TaskStatus>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = userService.UserId;
        if (userId is null)
            return Result<TaskStatus>.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<TaskStatus>.Fail(Error.NotFound("Task.NotFound",
                $"Task with id '{request.TaskId}' was not found."));

        // ownership الأو
        if (task.CreatedByUserId != userId)
            return Result<TaskStatus>.Fail(Error.Forbidden(
                "Task.Forbidden", "You are not allowed to modify this task."));

        // business rule
        if (!task.CanChangeStatusTo(request.Status))
            return Result<TaskStatus>.Fail(Error.Conflict("Task.InvalidStatusTransition",
                $"Cannot change task status from '{task.Status}' to '{request.Status}'."));

        task.ChangeStatus(request.Status);
        task.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TaskStatus>.Ok(task.Status);
    }
}