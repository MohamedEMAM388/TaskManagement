using Domain.Entities.Enums;

namespace Application.Features.Projects.Queries.GetProjects;

public class ProjectDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; }
}