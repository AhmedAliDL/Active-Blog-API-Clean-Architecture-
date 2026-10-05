using FluentValidation;

namespace App.Application.Roles.Command.CreateRole
{
    public class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleValidator()
        {
            RuleFor(r => r.RoleName)
                .MinimumLength(2)
                .WithMessage("Role name minimum length is 2")
                .MaximumLength(20)
                .WithMessage("Role name maximum length is 20");
            RuleFor(r => r.RoleDescription)
                .MinimumLength(5)
                .WithMessage("Role description name minimum length is 5")
                .MaximumLength(50)
                .WithMessage("Role description name maximum length is 50");
        }
    }
}
