using FluentValidation;

namespace App.Application.Roles.Command.DeleteRole
{
    public class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleValidator()
        {

            RuleFor(r => r.RoleName)
                .NotEmpty()
                .NotNull()
                .WithMessage("role name is required.");
        }
    }
}
