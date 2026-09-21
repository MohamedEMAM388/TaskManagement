namespace Application.Features.Tasks.Dtos;

public sealed record TaskDto(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime DueDate,
    int ProjectId,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
