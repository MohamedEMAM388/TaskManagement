using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Commands.Create;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    DateTime DueDate,
    int ProjectId) : IRequest<Result<int>>;
