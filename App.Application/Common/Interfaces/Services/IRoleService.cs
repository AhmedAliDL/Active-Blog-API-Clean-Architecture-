using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Common.Interfaces.Services
{
    public interface IRoleService : IScopedServiceMarker
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<IList<string>> GetAllRolesOfUser(User user);
        Task<Role?> GetRoleAsync(string roleName);
        Task<IdentityResult> CreateRoleAsync(string roleName, string roleDescription);
        Task<IdentityResult> AssignRoleToUserAsync(User user, string roleName);
        Task<bool> DeleteRoleAsync(Role role);
        Task<bool> DeleteAssignRoleAsync(User user, string roleName);
        Task<bool> IsRoleExistsAsync(string roleName);


    }
}
