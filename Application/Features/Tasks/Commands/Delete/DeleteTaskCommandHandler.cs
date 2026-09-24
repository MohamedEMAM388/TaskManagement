using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Tasks.Commands.Delete;

public class DeleteTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService) : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        // load the task with its comments so they are softly deleted with it
        var task = await unitOfWork.TaskRepository.GetTaskByIdWithCommentsAsync(request.Id, cancellationToken);
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
                "Task.Forbidden", "You are not allowed to delete this task."));

        task.SoftDelete(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}