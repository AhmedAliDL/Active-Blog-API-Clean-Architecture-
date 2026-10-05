using FluentValidation;
namespace App.Application.Auth.Command.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(p => p.FName)
               .MaximumLength(30)
               .WithMessage("First Name must be between 3 and 30 characters.")
               .MinimumLength(3)
               .WithMessage("First Name must be between 3 and 30 characters.");
            RuleFor(p => p.LName)
                .MaximumLength(30)
                .WithMessage("Last Name must be between 3 and 30 characters.")
                .MinimumLength(3)
                .WithMessage("Last Name must be between 3 and 30 characters.");
            RuleFor(p => p.Phone)
                .Matches(@"^01[0125]\d{8}$")
                .WithMessage("Invalid egyptian phone number.");
            RuleFor(p => p.Email)
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
