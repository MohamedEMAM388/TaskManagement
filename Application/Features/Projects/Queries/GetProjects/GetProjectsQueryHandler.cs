using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(IUnitOfWork unitOfWork , IMapper mapper) : 
       IRequestHandler<GetProjectsQuery , IEnumerable<ProjectDto>>
{

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        // get projects 
        var projects = await unitOfWork.ProjectRepository.GetAllAsync(cancellationToken);
        if (!projects.Any())
            return [];
        return mapper.Map<IEnumerable<ProjectDto>>(projects);

    }
}