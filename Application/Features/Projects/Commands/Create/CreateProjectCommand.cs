using Domain.Entities.Enums;
using MediatR;

namespace Application.Features.Projects.Commands.Create;

public sealed record CreateProjectCommand (   
    string Name,
    string Description,
    ProjectStatus Status
    ) : IRequest<bool>;
