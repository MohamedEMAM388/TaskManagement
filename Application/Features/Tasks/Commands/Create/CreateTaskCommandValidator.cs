using FluentValidation;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ProjectId)
            .GreaterThan(0).WithMessage("A valid ProjectId is required.");
    }
}
