using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Notifications.Dto;
using MediatR;

namespace App.Application.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<GetUserNotificationsQuery, List<UserNotificationDto>>
    {
        public async Task<List<UserNotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            return [..(await unitOfWork.Notifications.FindAllAsync(n => n.ReceiverId == userId, cancellationToken))
                        .Select(n => new UserNotificationDto
                        {
                            NotificationId = n.Id,
                            Message = n.Message,
                            SenderId = userId !.Value,
                            Link = n.Link,
                        })];
        }
    }
}
