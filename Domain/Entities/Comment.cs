using Domain.Common;

namespace Domain.Entities;

public class Comment : BaseEntity<int> , IHasOwner
{
    public string Content { get; set; } = string.Empty;

    // Foreign Key
    public int TaskId { get; init; }

    // Navigation Property
    public Task Task { get; init; } = null!;
    
    // Connect users to comments 
    public string CreatedByUserId { get; set; } = string.Empty;
}
