using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Roles.Command.CreateRole
{
    public record CreateRoleCommand : IRequest<IdentityResult>
    {
        public string RoleName { get; set; } = null!;
        public string? RoleDescription { get; init; }
    }
}
