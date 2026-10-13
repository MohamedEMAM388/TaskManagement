using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Commands.Assign;

public sealed record AssignTaskCommand(
    int TaskId,
    string UserId) : IRequest<Result>;
