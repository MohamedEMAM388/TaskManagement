using Application.Common.Identity;
using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Application.Features.Comments.Commands.Delete;

public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IUserService userService,
    IAuthorizationService authorizationService) : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var user = userService.User;
        if (user is null)
            return Result.Fail(Error.Unauthorized(
                "User.NotAuthenticated", "User is not authenticated"));

        var comment = await unitOfWork.CommentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (comment is null)
            return Result.Fail(Error.NotFound(
                "Comment.NotFound",
                $"Comment with ID {request.Id} was not found"));

        var authResult = await authorizationService.AuthorizeAsync(user, comment, "ResourceOwner");
        if (!authResult.Succeeded)
            return Result.Fail(Error.Forbidden(
                "Comment.Forbidden", "You are not allowed to delete this comment."));

        await unitOfWork.CommentRepository.DeleteAsync(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}