namespace Domain.Entities;

public class Task : BaseEntity<int>
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;

    public DateTime DueDate { get; set; }

    // Foreign Key
    public int ProjectId { get; set; }

    // Navigation Property
    public Project Project { get; set; } = null!;

    // Navigation Property
    public ICollection<Comment> Comments { get; set; } = [];
}