using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IAuthorizationService authorizationService) : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdWithTasksAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Fail(Error.NotFound(
                "Project.NotFound",
                $"Project with ID {request.Id} was not found"));

        var user = userService.User;
        if (user is null)
            return Result.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        var authResult = await authorizationService.AuthorizeAsync(user, project, "ResourceOwner");
        if (!authResult.Succeeded)
            return Result.Fail(Error.Forbidden(
                "Project.Forbidden", "You are not allowed to delete this project."));

        project.SoftDelete(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}