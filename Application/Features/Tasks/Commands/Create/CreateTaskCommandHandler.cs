using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using DomainTask = Domain.Entities.Task;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IAuthorizationService authorizationService) : IRequestHandler<CreateTaskCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result<int>.Fail(Error.NotFound("Project.NotFound",
                $"Project with id '{request.ProjectId}' was not found."));

        if (!project.CanAcceptTasks)
            return Result<int>.Fail(Error.Conflict("Project.NotAcceptingTasks",
                $"Cannot add tasks to a project with status '{project.Status}'."));

        var user = userService.User;
        if (user is null)
            return Result<int>.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        // Only the project owner (or an Admin) can create tasks inside it
        var authResult = await authorizationService.AuthorizeAsync(user, project, "ResourceOwner");
        if (!authResult.Succeeded)
            return Result<int>.Fail(Error.Forbidden("Project.Forbidden",
                "You are not allowed to add tasks to this project."));

        var userId = userService.UserId!;

        var task = new DomainTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ProjectId = project.Id,
            CreatedByUserId = userId,
        };

        await unitOfWork.TaskRepository.CreateTaskAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(task.Id);
    }
}