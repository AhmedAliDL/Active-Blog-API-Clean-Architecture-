using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace App.Infrastructure.Services
{
    public class IdentityService(UserManager<User> userManager) : IIdentityService
    {
        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }
        public async Task<List<User>> GetUsersByIdsAsync(List<Guid> ids)
        {
            return await userManager.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<bool> CheckCorrectnessOfPasswordAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await userManager.UpdateAsync(user);
        }
        public bool CheckEmailUniqueness(string email)
        {
            return userManager.Users.Any(u => u.Email == email);
        }
        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        public async Task<string> GenerateForgetPasswordConfirmationTokenAsync(User user)
        {
            return await userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<bool> CheckEmailConfirmationTokenAsync(User user, string token)
        {
            var isValid = await userManager.ConfirmEmailAsync(user, token);
            return isValid.Succeeded;
        }
        public async Task<bool> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword)
        {
            var changed = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return changed.Succeeded;
        }
        public async Task<bool> ResetUserPasswordAsync(User user, string token, string newPassword)
        {
            var changed = await userManager.ResetPasswordAsync(user, token, newPassword);
            return changed.Succeeded;
        }
    }

}
