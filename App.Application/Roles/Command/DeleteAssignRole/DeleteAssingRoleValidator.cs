using FluentValidation;

namespace App.Application.Roles.Command.DeleteAssignRole
{
    public class DeleteAssingRoleValidator : AbstractValidator<DeleteAssignRoleCommand>
    {
        public DeleteAssingRoleValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .NotNull()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Email address is not valid.");

            RuleFor(r => r.RoleName)
                .NotEmpty()
                .NotNull()
                .WithMessage("role name is required.");
        }
    }
}
