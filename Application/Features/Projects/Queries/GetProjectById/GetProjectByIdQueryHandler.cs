using Application.Common.ResultPattern;
using Application.Features.Projects.DTOS;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(request.Id, cancellationToken);
        if (project is null)
            return Result<ProjectDto>.Fail(ProjectErrors.NotFound(request.Id));

        return Result<ProjectDto>.Ok(mapper.Map<ProjectDto>(project));
    }
}
