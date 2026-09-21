using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Projects.Commands.Delete;

public sealed record DeleteProjectCommand(int Id) : IRequest<Result>;
