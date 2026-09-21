using Application.Common.ResultPattern;
using Application.Contracts;
using Application.Features.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Features.Comments.Commands.Add;

public class AddCommentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddCommentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<int>.Fail(TaskErrors.NotFound(request.TaskId));

        var comment = new Comment
        {
            Content = request.Content,
            TaskId = task.Id
        };

        await unitOfWork.CommentRepository.AddAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(comment.Id);
    }
}
