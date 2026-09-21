using FluentValidation;

namespace Application.Features.Comments.Commands.Delete;

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
