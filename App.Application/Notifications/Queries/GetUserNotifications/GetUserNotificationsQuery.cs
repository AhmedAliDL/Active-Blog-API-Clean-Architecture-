using App.Application.Notifications.Dto;
using MediatR;

namespace App.Application.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQuery : IRequest<List<UserNotificationDto>>;
}
