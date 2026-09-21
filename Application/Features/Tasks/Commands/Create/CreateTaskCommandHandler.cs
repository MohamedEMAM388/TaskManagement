using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Projects;
using DomainTask = Domain.Entities.Task;
using MediatR;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTaskCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Global query filter already excludes soft-deleted projects, so a null
        // result here covers both "doesn't exist" and "was deleted".
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result<int>.Fail(ProjectErrors.NotFound(request.ProjectId));

        var task = new DomainTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            ProjectId = project.Id
        };

        await unitOfWork.TaskRepository.CreateTaskAsync(task, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(task.Id);
    }
}
