using FluentValidation;

namespace App.Application.Roles.Command.AssignRole
{
    public class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleValidator()
        {
            RuleFor(r => r.UserEmail)
                .EmailAddress()
               .WithMessage("Invalid Email address.")
               .Must(email =>
                    email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@yahoo.com", StringComparison.OrdinalIgnoreCase) ||
                    email.EndsWith("@hotmail.com", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Email must be a valid email address from gmail, yahoo, or hotmail domains.");
            RuleFor(r => r.RoleName)
                .MinimumLength(2)
                .WithMessage("Role name minimum length is 2")
                .MaximumLength(20)
                .WithMessage("Role name maximum length is 20");
        }
    }
}
