namespace Domain.Entities;

public class BaseEntity<TKey> 
{
    public TKey Id { get; set; } = default!;
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}