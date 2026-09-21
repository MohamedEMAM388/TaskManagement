using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Projects.Commands.Update;

public class UpdateProjectCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateProjectCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(request.Id));

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;
        project.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.ProjectRepository.UpdateAsync(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
