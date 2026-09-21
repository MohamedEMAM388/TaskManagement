using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteProjectCommand, Result>
{
    public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result.Fail(ProjectErrors.NotFound(request.Id));

        await unitOfWork.ProjectRepository.DeleteAsync(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
