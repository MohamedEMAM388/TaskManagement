using Application.Common.ResultPattern;
using Application.Features.Comments.Dtos;
using Application.Features.Tasks;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Comments.Queries.GetTaskComments;

public class GetTaskCommentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetTaskCommentsQuery, Result<List<CommentDto>>>
{
    public async Task<Result<List<CommentDto>>> Handle(GetTaskCommentsQuery request, CancellationToken cancellationToken)
    {
        // Confirms the task exists (and isn't soft-deleted) before listing its comments.
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result<List<CommentDto>>.Fail(Error.NotFound(
                "Task.NotFound",
                $"Task with ID {request.TaskId} was not found"));

        var comments = await unitOfWork.CommentRepository.GetByTaskIdAsync(request.TaskId, cancellationToken);
        return Result<List<CommentDto>>.Ok(mapper.Map<List<CommentDto>>(comments));
    }
}
