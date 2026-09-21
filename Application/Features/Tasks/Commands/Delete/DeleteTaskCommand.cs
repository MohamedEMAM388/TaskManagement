using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Tasks.Commands.Delete;

public sealed record DeleteTaskCommand(int Id) : IRequest<Result>;
