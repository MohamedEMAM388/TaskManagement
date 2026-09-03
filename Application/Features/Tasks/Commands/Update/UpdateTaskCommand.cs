using MediatR;

namespace Application.Features.Tasks.Commands.update;

public sealed record UpdateTaskCommand(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime DueDate,
    int ProjectId) : IRequest<bool>;
