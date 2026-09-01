using FluentValidation;

namespace Application.Features.Tasks.Commands.Create;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        
        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(256)
            .WithMessage("Title cannot exceed 256 characters");
        
        RuleFor(t => t.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(256)
            .WithMessage("Description cannot exceed 256 characters");

        RuleFor(t => t.DueDate)
            .NotEmpty()
            .WithMessage("DueDate is required")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("DueDate cannot exceed today's date");



    }
}