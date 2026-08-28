namespace Domain.Entities;

public class Task : BaseEntity<int>
{
   public DateTime CreateAt { get; set; } 
   public DateTime UpdateAt { get; set; }
}