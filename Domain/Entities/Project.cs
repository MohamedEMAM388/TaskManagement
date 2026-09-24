using Domain.Entities.Enums;

namespace Domain.Entities;

public class Project : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; }

    public ICollection<Task> Tasks { get; init; } = [];
    
    // Connect users to Projects 
    public string CreatedByUserId { get; set; } = string.Empty;
    
    ////////////////
    public void SoftDelete(DateTime deletedAt)
    {
        foreach (var task in Tasks)
           task.SoftDelete(deletedAt);
        
        MarkAsDeleted(deletedAt);
        
    }
    
    // A cancelled or archived project can't receive new tasks
    public bool CanAcceptTasks =>
        Status is not (ProjectStatus.Cancelled or ProjectStatus.Archived);
}
