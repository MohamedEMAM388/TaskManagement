using FluentValidation;

namespace Application.Features.Comments.Commands.Add;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required.")
            .MaximumLength(1000);

        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("A valid TaskId is required.");
    }
}
