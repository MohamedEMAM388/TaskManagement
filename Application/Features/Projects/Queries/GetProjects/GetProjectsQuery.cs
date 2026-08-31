using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery() : IRequest<IEnumerable<ProjectDto>>;
