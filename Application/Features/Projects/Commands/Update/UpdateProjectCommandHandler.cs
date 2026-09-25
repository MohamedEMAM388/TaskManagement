using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Projects.Commands.Update;

public class UpdateProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IAuthorizationService authorizationService) : IRequestHandler<UpdateProjectCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
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
                "Project.Forbidden", "You are not allowed to modify this project."));

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;
        project.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.ProjectRepository.UpdateAsync(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}