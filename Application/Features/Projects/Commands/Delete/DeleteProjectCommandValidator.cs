using FluentValidation;

namespace Application.Features.Projects.Commands.Delete;

public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
