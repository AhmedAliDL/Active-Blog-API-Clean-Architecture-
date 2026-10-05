using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Roles.Command.AssignRole
{
    public record AssignRoleCommand : IRequest<IdentityResult>
    {
        public string UserEmail { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}
