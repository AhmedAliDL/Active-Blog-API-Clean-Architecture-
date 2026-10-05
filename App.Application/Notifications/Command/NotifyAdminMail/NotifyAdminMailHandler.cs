using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Notifications.Dto;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace App.Application.Notifications.Command.NotifyAdminMail
{
    public class NotifyAdminMailHandler(IContactService contactService, ICurrentUserService currentUserService, IIdentityService identityService, IConfiguration configuration, ILogger<NotifyAdminMailHandler> logger) : IRequestHandler<NotifyAdminMailCommand, NotifyDto>
    {
        public async Task<NotifyDto> Handle(NotifyAdminMailCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Notify admin operation started.");
            Guid? userId = currentUserService.UserId ?? throw new NotFoundException("User not found");
            User? user = await identityService.GetUserByIdAsync(userId!.Value);
            if (user == null)
                throw new NotFoundException("User not found");
            string emailBody = $"From: {user!.Email}\n\n" +
                            $"User Name:{user.FName + ' ' + user.LName}\n\n" +
                          $"Subject: {request.Title}\n\n" +
                          $"{request.Message}";

            var res = await contactService.SendEmailToAdminServiceAsync(configuration["SmtpSettings:AdminEmail"]!, user.Email!, request.Title, emailBody, "User", "Active Blog");
            logger.LogInformation("Notify admin operation started.");
            return res;
        }
    }
}
