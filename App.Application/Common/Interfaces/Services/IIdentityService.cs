using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Common.Interfaces.Services
{
    public interface IIdentityService : IScopedServiceMarker
    {
        Task<List<User>> GetUsersByIdsAsync(List<Guid> ids);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<IdentityResult> UpdateUserAsync(User user);

        bool CheckEmailUniqueness(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<bool> CheckEmailConfirmationTokenAsync(User user, string token);

        Task<bool> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword);
        Task<string> GenerateForgetPasswordConfirmationTokenAsync(User user);
        Task<bool> ResetUserPasswordAsync(User user, string token, string newPassword);
        Task<bool> CheckCorrectnessOfPasswordAsync(User user, string password);
    }
}
