using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Features.Tasks.DTOs;
using Application.Contracts;
using AutoMapper;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler(IUnitOfWork unitOfWork, IMapper mapper ,
    IUserService userService)
    : IRequestHandler<GetTasksQuery, Result<List<TaskDto>>>
{
    public async Task<Result<List<TaskDto>>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var isAdmin = userService.User?.IsInRole("Admin") ?? false;
        var tasks = await unitOfWork.TaskRepository
                        .GetAllAsync(isAdmin ? null :request.OwnerId,cancellationToken);
        return Result<List<TaskDto>>.Ok(mapper.Map<List<TaskDto>>(tasks));
    }
}
