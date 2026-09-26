using Application.Features.Tasks.DTOs;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(string OwnerId) : IRequest<Result<List<TaskDto>>>;
