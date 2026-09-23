using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Commands.Update;

public sealed record UpdateTaskCommand(
    int Id,
    string Title,
    string Description,
    DateTime DueDate) : IRequest<Result>;
