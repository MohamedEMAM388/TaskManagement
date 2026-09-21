using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Comments.Commands.Delete;

public sealed record DeleteCommentCommand(int Id) : IRequest<Result>;
