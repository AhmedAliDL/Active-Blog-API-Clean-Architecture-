using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace App.Application.Roles.Command.AssignRole
{
    public class AssignRoleHandler(IIdentityService identityService, IRoleService roleService, ILogger<AssignRoleHandler> logger) : IRequestHandler<AssignRoleCommand, IdentityResult>
    {
        public async Task<IdentityResult> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Assign role operation started.");
            request.RoleName = request.RoleName.ToLower();
            User? user = await identityService.GetUserByEmailAsync(request.UserEmail);
            bool foundRole = await roleService.IsRoleExistsAsync(request.RoleName);
            if (!foundRole) throw new NotFoundException("Role not found");
            var result = user == null
                ? throw new NotFoundException("User not found.")
                : await roleService.AssignRoleToUserAsync(user, request.RoleName);
            logger.LogInformation("Assign role operation completed.");
            return result;
        }
    }
}
