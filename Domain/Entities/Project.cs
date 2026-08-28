namespace Domain.Entities;

public class Project : BaseEntity<int>
{
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}