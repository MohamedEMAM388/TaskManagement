using Application.Features.Projects.DTOS;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(int Id) : IRequest<Result<ProjectDto>>;
