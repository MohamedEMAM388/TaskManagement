using Domain.Entities.Enums;

namespace Domain.Entities;

public class Project : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public ProjectStatus Status { get; set; }

    public ICollection<Task> Tasks { get; set; } = [];
}