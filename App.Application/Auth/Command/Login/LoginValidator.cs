using FluentValidation;

namespace App.Application.Auth.Command.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Email)
                .EmailAddress()
                .WithMessage("Invalid email address.")
                .Must(email =>
                    email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@yahoo.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@hotmail.com", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Email must be a valid email address from gmail, yahoo, or hotmail domains.");
        }
    }
}
