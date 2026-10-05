using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Services
{
    public class RoleService(RoleManager<Role> roleManager, UserManager<User> userManager) : IRoleService
    {

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = await roleManager.Roles.ToListAsync();

            return roles;

        }
        public async Task<IList<string>> GetAllRolesOfUser(User user)
        {
            return await userManager.GetRolesAsync(user);
        }
        public async Task<bool> IsRoleExistsAsync(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }
        public async Task<IdentityResult> CreateRoleAsync(string roleName, string roleDescription)
        {

            var role = new Role
            {
                Name = roleName,
                RoleDescription = roleDescription
            };
            var result = await roleManager.CreateAsync(role);
            return result;
        }
        public async Task<IdentityResult> AssignRoleToUserAsync(User user, string roleName)
        {

            var result = await userManager.AddToRoleAsync(user, roleName);
            return result;
        }
        public async Task<bool> DeleteAssignRoleAsync(User user, string roleName)
        {
            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }
        public async Task<Role?> GetRoleAsync(string roleName)
        {
            return await roleManager.FindByNameAsync(roleName);
        }
        public async Task<bool> DeleteRoleAsync(Role role)
        {

            var result = await roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

    }
}
