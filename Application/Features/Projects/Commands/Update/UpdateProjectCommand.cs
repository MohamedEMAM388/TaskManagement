using Domain.Entities.Enums;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Projects.Commands.Update;

public sealed record UpdateProjectCommand(
    int Id,
    string Name,
    string Description,
    ProjectStatus Status) : IRequest<Result>;
