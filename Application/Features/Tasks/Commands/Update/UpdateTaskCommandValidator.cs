using FluentValidation;

namespace Application.Features.Tasks.Commands.Update;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
