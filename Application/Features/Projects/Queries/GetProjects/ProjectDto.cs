using Domain.Entities.Enums;

namespace Application.Features.Projects.Queries.GetProjects;

public class ProjectDto
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public ProjectStatus Status { get; init; }
    
    public DateTime StartAt { get; init; }
}