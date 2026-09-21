using Application.Common.ResultPattern;
using MediatR;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateStatus;

public sealed record UpdateStatusCommand(
    int TaskId , 
    TaskStatus Status) 
    : IRequest<Result<TaskStatus>>;
