using Application.Common.ResultPattern;

namespace Application.Features.Comments;

public static class CommentErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Comment.NotFound", $"Comment with id '{id}' was not found.");
}
