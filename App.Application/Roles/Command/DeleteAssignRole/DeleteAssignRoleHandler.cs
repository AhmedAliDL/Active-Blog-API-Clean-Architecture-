using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Roles.Command.DeleteAssignRole
{
    public class DeleteAssignRoleHandler(IIdentityService identityService, IRoleService roleService, ILogger<DeleteAssignRoleHandler> logger) : IRequestHandler<DeleteAssignRoleCommand, bool>
    {
        public async Task<bool> Handle(DeleteAssignRoleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete user role operation started.");
            User? user = await identityService.GetUserByEmailAsync(request.Email) ?? throw new NotFoundException("User not found.");
            request.RoleName = request.RoleName.ToLower();
            bool found = await roleService.IsRoleExistsAsync(request.RoleName);
            if (!found) throw new NotFoundException("Role not found.");
            var res = await roleService.DeleteAssignRoleAsync(user, request.RoleName);
            logger.LogInformation("Delete user role operation started.");
            return res;
        }
    }
}
