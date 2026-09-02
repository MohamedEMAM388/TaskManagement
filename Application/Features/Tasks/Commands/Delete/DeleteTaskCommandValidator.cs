using FluentValidation;

namespace Application.Features.Tasks.Commands.Delete;

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public  DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Project id must be greater than zero");
        
    }
    
}