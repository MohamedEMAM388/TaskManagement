using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Comments.Commands.Add;

public sealed record AddCommentCommand(
    string Content,
    int TaskId) : IRequest<Result<int>>;
