using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainTask = Domain.Entities.Task;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<DomainTask>
{
    public void Configure(EntityTypeBuilder<DomainTask> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);
        
        builder.Property(t => t.Status)
            .HasConversion<string>()  
            .HasMaxLength(20)
            .HasDefaultValue(TaskStatus.Todo)
            .IsRequired();

        builder.Property(t => t.DueDate)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeletedAt);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Task)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}