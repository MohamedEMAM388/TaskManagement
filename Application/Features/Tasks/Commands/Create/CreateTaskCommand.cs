using MediatR;

namespace Application.Features.Tasks.Commands.Create;

public sealed record CreateTaskCommand
(
    string Title,
    string Description,
    DateTime DueDate) : IRequest<int>;
