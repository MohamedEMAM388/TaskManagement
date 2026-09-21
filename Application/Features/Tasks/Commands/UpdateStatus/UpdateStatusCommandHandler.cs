using Application.Common.ResultPattern;
using Application.Contracts;
using Domain.Exceptions;
using MediatR;
using TaskStatus = Domain.Entities.Enums.TaskStatus;

namespace Application.Features.Tasks.Commands.UpdateStatus;

public class UpdateStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateStatusCommand, Result<TaskStatus>>
{
    public async Task<Result<TaskStatus>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<TaskStatus>.Fail(TaskErrors.NotFound(request.TaskId));

        // The domain entity enforces the allowed transitions by throwing;
        // we translate that into a Result so callers never see the exception.
        try
        {
            task.ChangeStatus(request.Status);
        }
        catch (InvalidTaskStatusTransitionException ex)
        {
            return Result<TaskStatus>.Fail(TaskErrors.InvalidStatusTransition(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TaskStatus>.Ok(task.Status);
    }
}
