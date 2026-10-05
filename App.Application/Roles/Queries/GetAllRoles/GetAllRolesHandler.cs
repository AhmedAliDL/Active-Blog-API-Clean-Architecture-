using App.Application.Common.Interfaces.Services;
using App.Application.Roles.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Roles.Queries.GetAllRoles
{
    public class GetAllRolesHandler(IRoleService roleService, ILogger<GetAllRolesHandler> logger) : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all roles operation started.");
            var roles = await roleService.GetAllRolesAsync();
            List<RoleDto> result = [..roles.Select(r =>
            {
                return new RoleDto
                {
                    RoleName = r.Name!,
                    RoleDescription = r.NormalizedName
                };
            })];
            logger.LogInformation("Get all roles operation completed.");
            return result;
        }
    }
}
