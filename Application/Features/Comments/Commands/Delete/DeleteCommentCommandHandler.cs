using Application.Common.ResultPattern;
using Application.Contracts;
using MediatR;

namespace Application.Features.Comments.Commands.Delete;

public class DeleteCommentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await unitOfWork.CommentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (comment is null)
            return Result.Fail(CommentErrors.NotFound(request.Id));

        await unitOfWork.CommentRepository.DeleteAsync(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
