using FluentValidation;

namespace App.Application.Auth.Command.ForgetPassword
{
    public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordValidator()
        {
            RuleFor(p => p.Email)
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
