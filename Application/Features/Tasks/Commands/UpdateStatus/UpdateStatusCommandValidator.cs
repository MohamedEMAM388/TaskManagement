using FluentValidation;

namespace Application.Features.Tasks.Commands.UpdateStatus;

public class UpdateStatusCommandValidator : AbstractValidator<UpdateStatusCommand>
{
    public UpdateStatusCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .GreaterThan(0).WithMessage("A valid TaskId is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid task status.");
    }
}