using Application.Common.ResultPattern;
using Application.Features.Tasks.DTOs;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await unitOfWork.TaskRepository.GetTaskByIdAsync(request.Id, cancellationToken);
        if (task is null)
            return Result<TaskDto>.Fail(Error.NotFound(
                "Task.NotFound",
                $"Task with ID {request.Id} was not found"));

        return Result<TaskDto>.Ok(mapper.Map<TaskDto>(task));
    }
}
