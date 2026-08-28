namespace Domain.Entities;

public class Comment : BaseEntity<int>
{
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}