using FluentValidation;

namespace Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        
        RuleFor(lc => lc.LoginDto.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");
        
        RuleFor(lc => lc.LoginDto.Password)
            .NotEmpty()
            .WithMessage("Password is required");
        
    }
}