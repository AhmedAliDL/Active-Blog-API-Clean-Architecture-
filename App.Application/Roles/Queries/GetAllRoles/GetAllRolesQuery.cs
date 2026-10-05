using App.Application.Roles.Dto;
using MediatR;

namespace App.Application.Roles.Queries.GetAllRoles
{
    public record GetAllRolesQuery : IRequest<List<RoleDto>>;
}
