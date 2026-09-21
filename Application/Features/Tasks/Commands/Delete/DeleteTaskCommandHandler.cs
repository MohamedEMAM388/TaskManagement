using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Tasks.Commands.Delete;

public class DeleteTaskCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteTaskCommand, Result>
{
    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result.Fail(TaskErrors.NotFound(request.Id));

        await unitOfWork.TaskRepository.DeleteTaskAsync(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
