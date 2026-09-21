using Application.Features.Comments.Dtos;
using Application.Common.ResultPattern;
using MediatR;

namespace Application.Features.Comments.Queries.GetTaskComments;

public sealed record GetTaskCommentsQuery(int TaskId) : IRequest<Result<List<CommentDto>>>;
