using Application.Features.Tasks.Dtos;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(int Id) : IRequest<Result<TaskDto>>;
