using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Domain.Exceptions;

public class InvalidTaskStatusTransitionException(TaskStatus from, TaskStatus to)
    : Exception($"Cannot change task status from '{from}' to '{to}'.");