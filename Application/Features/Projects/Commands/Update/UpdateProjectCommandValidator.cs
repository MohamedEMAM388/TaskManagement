using FluentValidation;

namespace Application.Features.Projects.Commands.Update;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
