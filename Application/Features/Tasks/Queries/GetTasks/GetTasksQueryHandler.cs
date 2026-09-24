using Application.Common.ResultPattern;
using Application.Features.Tasks.DTOs;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetTasksQuery, Result<List<TaskDto>>>
{
    public async Task<Result<List<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await unitOfWork.TaskRepository.GetAllAsync(cancellationToken);
        return Result<List<TaskDto>>.Ok(mapper.Map<List<TaskDto>>(tasks));
    }
}
