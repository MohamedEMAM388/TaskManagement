namespace Domain.Entities;

public class Comment : BaseEntity<int>
{
    public string Content { get; set; } = string.Empty;

    // Foreign Key
    public int TaskId { get; set; }

    // Navigation Property
    public Task Task { get; set; } = null!;
}