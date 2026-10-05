using MediatR;

namespace App.Application.Roles.Command.DeleteRole
{
    public record DeleteRoleCommand : IRequest<bool>
    {
        public string RoleName { get; set; } = string.Empty;
    }
}
