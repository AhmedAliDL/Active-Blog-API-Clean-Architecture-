using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace App.Application.Roles.Command.CreateRole
{
    public class CreateRoleHandler(IRoleService roleService, ILogger<CreateRoleHandler> logger) : IRequestHandler<CreateRoleCommand, IdentityResult>
    {
        private readonly IRoleService roleService = roleService;

        public async Task<IdentityResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create role operation started.");
            request.RoleName = request.RoleName.ToLower();
            var res = await roleService.CreateRoleAsync(request.RoleName, request.RoleDescription!);
            logger.LogInformation("Create role operation completed.");
            return res;
        }
    }
}
