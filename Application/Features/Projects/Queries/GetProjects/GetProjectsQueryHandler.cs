using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Features.Projects.DTOS;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper ,
    IUserService userService)
    : IRequestHandler<GetProjectsQuery, Result<List<ProjectDto>>>
{
    public async Task<Result<List<ProjectDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var isAdmin = userService.User?.IsInRole("Admin") ?? false;
        var projects = await unitOfWork.ProjectRepository
            .GetAllAsync(isAdmin ?  null : request.OwnerId ,cancellationToken);
        return Result<List<ProjectDto>>.Ok(mapper.Map<List<ProjectDto>>(projects));
    }
}
