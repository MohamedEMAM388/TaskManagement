using Domain.Exceptions;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Domain.Entities;

public class Task : BaseEntity<int>
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    public DateTime DueDate { get; set; }

    // Foreign Key
    public int ProjectId { get; init; }

    // Navigation Property
    public Project Project { get; init; } = null!;

    // Navigation Property
    public ICollection<Comment> Comments { get; init; } = [];
    
    // Connect users to tasks 
    public string CreatedByUserId { get; set; } = string.Empty;
    
    // 
    private static readonly Dictionary<TaskStatus, TaskStatus[]> StatusTransitions = new()
    {

        [TaskStatus.Todo] =
        [
            TaskStatus.InProgress,
            TaskStatus.Cancelled
        ],

        [TaskStatus.InProgress] =
        [
            TaskStatus.Completed,
            TaskStatus.Cancelled
        ],

        [TaskStatus.Completed] = [],

        [TaskStatus.Cancelled] = []

    };

    public bool CanChangeStatusTo(TaskStatus newStatus) =>
        StatusTransitions.TryGetValue(Status, out var allowedStatuses)
        && allowedStatuses.Contains(newStatus);

    public void ChangeStatus(TaskStatus newStatus)
    {
        if (!CanChangeStatusTo(newStatus))
            throw new InvalidTaskStatusTransitionException(Status, newStatus);

        Status = newStatus;
    }
    
    ////////////////
    public void SoftDelete(DateTime deletedAt)
    {
        foreach (var comment in Comments)
            comment.MarkAsDeleted(deletedAt);
        
        MarkAsDeleted(deletedAt);
        
    }
    
    // A completed or cancelled task is closed and can't be edited
    public bool IsClosed => Status is TaskStatus.Cancelled or TaskStatus.Completed;

}


