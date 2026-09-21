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
    
    // private methods
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

    public void ChangeStatus(TaskStatus newStatus)
    {
        if (!StatusTransitions.TryGetValue(Status, out var allowedStatuses)
            || !allowedStatuses.Contains(newStatus))
        {
            throw new InvalidTaskStatusTransitionException(
                Status,
                newStatus);
        }

        Status = newStatus;
    }

}


