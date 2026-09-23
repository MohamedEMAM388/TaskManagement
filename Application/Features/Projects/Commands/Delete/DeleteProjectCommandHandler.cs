using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService) : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdWithTasksAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(request.Id));

        var userId = userService.UserId;
        if (userId is null)
            return Result.Fail(Error.Validation(
                "User.NotAuthenticated", "User is not authenticated"));

        if (project.CreatedByUserId != userId)
            return Result.Fail(Error.Forbidden(
                "Project.Forbidden", "You are not allowed to delete this project."));

        project.SoftDelete(DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}