using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Notifications.Dto;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace App.Application.Auth.Command.SendEmailConfirmation
{
    public class SendEmailConfirmationHandler(IIdentityService identityService, IContactService contactService, IConfiguration configuration, ILogger<SendEmailConfirmationHandler> logger) : IRequestHandler<SendEmailConfirmationCommand, NotifyDto>
    {
        public async Task<NotifyDto> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Send email confirmation token operation started.");
            User user = await identityService.GetUserByEmailAsync(request.Email) ?? throw new NotFoundException("User not found.");
            string token = await identityService.GenerateEmailConfirmationTokenAsync(user);
            var res = await contactService.SendEmailServiceAsync(
                 user.Email!,
                  configuration["SmtpSettings:AdminEmail"]!,
                 "Email Confirmation Code",
                 $"Your confirmation code is: {token}",
                 "Active Blog",
                 "User"
             );
            logger.LogInformation("Send email confirmation token operation started.");
            return res;
        }
    }
}
