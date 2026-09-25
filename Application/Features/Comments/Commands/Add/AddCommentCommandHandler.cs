using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using Domain.Entities;
using MediatR;

namespace Application.Features.Comments.Commands.Add;

public class AddCommentCommandHandler(IUnitOfWork unitOfWork,
    IUserService userService) : IRequestHandler<AddCommentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = userService.UserId;
        if (userId is null)
            return Result<int>.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));
        
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<int>.Fail(Error.NotFound("Task.NotFound",
                $"Task with id '{request.TaskId}' was not found."));

        if (task.IsClosed)
            return Result<int>.Fail(Error.Conflict("Task.AlreadyClosed",
                $"Cannot modify a task that is {task.Status}."));
        

        // The task owner or the owner of the parent project can comment
        var project = await unitOfWork.ProjectRepository.GetByIdAsync(task.ProjectId, cancellationToken);
        if (project is null)
            return Result<int>.Fail(Error.NotFound("Project.NotFound",
                $"Project with id '{task.ProjectId}' was not found."));
        
        var canComment = task.CreatedByUserId == userId
                         || project.CreatedByUserId == userId;

        if (!canComment)
            return Result<int>.Fail(Error.Forbidden("Comment.Forbidden",
                "You are not allowed to comment on this task."));
        var comment = new Comment
        {
            Content = request.Content,
            TaskId = task.Id,
            CreatedByUserId = userId,
        };

        await unitOfWork.CommentRepository.AddAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Ok(comment.Id);
    }
}
