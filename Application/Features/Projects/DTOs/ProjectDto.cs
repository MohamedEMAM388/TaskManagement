using Domain.Entities.Enums;

namespace Application.Features.Projects.DTOS;

public sealed record ProjectDto(
    int Id,
    string Name,
    string Description,
    ProjectStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
