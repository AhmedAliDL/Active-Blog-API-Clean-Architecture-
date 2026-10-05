using MediatR;

namespace App.Application.Roles.Command.DeleteAssignRole
{
    public record DeleteAssignRoleCommand : IRequest<bool>
    {
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
