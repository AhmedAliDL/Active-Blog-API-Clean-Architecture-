using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Roles.Command.DeleteRole
{
    public class DeleteRoleHandler(IRoleService roleService, ILogger<DeleteRoleHandler> logger) : IRequestHandler<DeleteRoleCommand, bool>
    {

        async Task<bool> IRequestHandler<DeleteRoleCommand, bool>.Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete role operation started.");
            request.RoleName = request.RoleName.ToLower();
            Role? role = await roleService.GetRoleAsync(request.RoleName) ?? throw new NotFoundException("Role not found.");
            var res = await roleService.DeleteRoleAsync(role);
            logger.LogInformation("Delete role operation completed.");
            return res;

        }
    }
}
