namespace Application.Features.Tasks.DTOs;

public sealed record TaskDto(
    int Id,
    string Title,
    string Description,
    DateTime DueDate,
    int ProjectId,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
