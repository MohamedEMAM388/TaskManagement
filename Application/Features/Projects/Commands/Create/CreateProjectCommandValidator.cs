using FluentValidation;

namespace Application.Features.Projects.Commands.Create;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
        
        RuleFor(y => y.Description)
            .NotEmpty()
            .WithMessage("Description is required");

    }

}