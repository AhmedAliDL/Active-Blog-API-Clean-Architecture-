using FluentValidation;

namespace App.Application.Auth.Command.SendEmailConfirmation
{
    public class SendEmailConfirmationValidator : AbstractValidator<SendEmailConfirmationCommand>
    {
        public SendEmailConfirmationValidator()
        {
            RuleFor(e => e.Email)
                .EmailAddress()
                .WithMessage("Email format is not valid.")
                .Must(email =>
                    email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@yahoo.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@hotmail.com", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Email must be a valid email address from gmail, yahoo, or hotmail domains.");
        }
    }
}
