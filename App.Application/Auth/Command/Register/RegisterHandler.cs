using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Auth.Command.Register
{
    public class RegisterHandler(IIdentityService identityService, IRoleService roleService, ILogger<RegisterHandler> logger) : IRequestHandler<RegisterCommand, Guid>
    {
        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("User registration operation started.");
            var user = new User
            {
                FName = request.FName,
                Email = request.Email,
                LName = request.LName,
                PhoneNumber = request.Phone,
                Address = request.Address,
                UserName = request.Email,
                Image = request.ImagePath
            };

            var result = await identityService.CreateUserAsync(user, request.Password);
            if (result.Succeeded)
            {
                bool found = await roleService.IsRoleExistsAsync("user");
                if (found)
                    await roleService.AssignRoleToUserAsync(user, "user");
                else
                {
                    var res = await roleService.CreateRoleAsync("user", "normal user for the system.");
                    if (res.Succeeded)
                        await roleService.AssignRoleToUserAsync(user, "user");
                    else
                    {
                        throw new InvalidOperationException("Failed to create user");
                    }

                }
            }

            logger.LogInformation("User registration operation completed.");
            return user.Id;
        }
    }
}
