using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Tasks.Commands.Update;

public class UpdateTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateTaskCommand, Result>
{
    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Fail(Error.NotFound(
                "Task.NotFound",
                $"Task with ID {request.Id} was not found"));

        var user = userService.User;
        if (user is null)
            return Result.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        var authResult = await authorizationService.AuthorizeAsync(user, task, "ResourceOwner");
        if (!authResult.Succeeded)
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