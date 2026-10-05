using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Notifications.Dto;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace App.Application.Auth.Command.ForgetPassword
{
    public class ForgetPasswordHandler(IConfiguration configuration, IContactService contactService, IIdentityService identityService, ILogger<ForgetPasswordHandler> logger) : IRequestHandler<ForgetPasswordCommand, NotifyDto>
    {
        public async Task<NotifyDto> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Forget password operation started.");
            User user = await identityService.GetUserByEmailAsync(request.Email) ?? throw new NotFoundException("User not found.");
            string token = await identityService.GenerateForgetPasswordConfirmationTokenAsync(user);
            var result = await contactService.SendEmailServiceAsync(
                 user.Email!,
                  configuration["SmtpSettings:AdminEmail"]!,
                 "Forget Password Confirmation Code",
                 $"Your confirmation code is: {token}",
                 "Active Blog",
                 "User"
             );
            logger.LogInformation("Forget password operation completed.");
            return result;
        }
    }
}
