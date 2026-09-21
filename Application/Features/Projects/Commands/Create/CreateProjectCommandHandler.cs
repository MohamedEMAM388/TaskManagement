using Application.Common.ResultPattern;
using Application.Contracts;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status
        };

        await unitOfWork.ProjectRepository.CreateAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(project.Id);
    }
}
