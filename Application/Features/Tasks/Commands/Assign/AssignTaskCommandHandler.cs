using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Tasks.Commands.Assign;

public class AssignTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IIdentityService identityService,
    IAuthorizationService authorizationService) : IRequestHandler<AssignTaskCommand, Result>
{
    public async Task<Result> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var user = userService.User;
        if (user is null)
            return Result.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result.Fail(Error.NotFound("Task.NotFound",
                $"Task with id '{request.TaskId}' was not found."));

        // Only the task owner (or an Admin) can assign it
        var authResult = await authorizationService.AuthorizeAsync(user, task, "ResourceOwner");
        if (!authResult.Succeeded)
            return Result.Fail(Error.Forbidden(
                "Task.Forbidden", "You are not allowed to assign this task."));

        // A completed or cancelled task is closed and can't be changed
        if (task.IsClosed)
            return Result.Fail(Error.Conflict("Task.AlreadyClosed",
                $"Cannot modify a task that is {task.Status}."));

        // The assignee must be an existing user (NotFound error if it doesn't exist)
        var assignee = await identityService.GetUserByIdAsync(request.UserId);
        if (!assignee.IsSuccess)
            return Result.Fail(assignee.Errors);

        task.AssignTo(assignee.Value.Id);
        task.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
