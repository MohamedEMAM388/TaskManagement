using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.Features.Tasks.DTOs;

public sealed record UpdateTaskStatusDto(TaskStatus Status);