using Application.Features.Projects.DTOS;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery() : IRequest<Result<List<ProjectDto>>>;
