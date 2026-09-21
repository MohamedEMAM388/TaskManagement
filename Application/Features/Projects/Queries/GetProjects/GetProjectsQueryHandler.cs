using Application.Common.ResultPattern;
using Application.Features.Projects.DTOS;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetProjectsQuery, Result<List<ProjectDto>>>
{
    public async Task<Result<List<ProjectDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await unitOfWork.ProjectRepository.GetAllAsync(cancellationToken);
        return Result<List<ProjectDto>>.Ok(mapper.Map<List<ProjectDto>>(projects));
    }
}
