using Application.Features.Tasks.Dtos;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery() : IRequest<Result<List<TaskDto>>>;
