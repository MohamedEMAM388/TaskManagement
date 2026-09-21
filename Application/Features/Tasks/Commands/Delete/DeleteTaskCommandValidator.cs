using FluentValidation;

namespace Application.Features.Tasks.Commands.Delete;

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
