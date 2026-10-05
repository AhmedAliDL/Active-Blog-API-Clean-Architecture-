using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace App.Infrastructure.Services
{
    public class NotificationService(IHubContext<NotificationHub> context, IUnitOfWork unitOfWork) : INotificationService
    {
        public async Task NotifyUserAsync(Guid reciver, string message, Guid sender, string? link)
        {
            await context.Clients.User(reciver.ToString())
                .SendAsync("newMessage", message, sender);
            await unitOfWork.Notifications.AddAsync(new Domain.Entities.Notification
            {
                ReceiverId = reciver,
                SenderId = sender,
                Message = message,
                IsRead = false,
                Link = link
            });
            await unitOfWork.CompleteAsync();
        }
    }

}
