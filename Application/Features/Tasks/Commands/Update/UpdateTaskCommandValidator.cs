using FluentValidation;

namespace Application.Features.Tasks.Commands.update;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(t => t.Id)
             .GreaterThan(0)
             .WithMessage("Id must be greater than 0");
        
        RuleFor(t => t.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(256)
            .WithMessage("Title cannot exceed 256 characters");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(256)
            .WithMessage("Description cannot exceed 256 characters");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("DueDate must be in the future");
        
        RuleFor(t => t.ProjectId)
            .GreaterThan(0)
            .WithMessage("ProjectId must be greater than 0");
    }
    
}