namespace Application.Features.Comments.Dtos;

public sealed record CommentDto(
    int Id,
    string Content,
    int TaskId,
    DateTime CreatedAt);
