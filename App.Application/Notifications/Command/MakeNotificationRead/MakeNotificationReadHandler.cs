using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;

namespace App.Application.Notifications.Command.MakeNotificationRead
{
    public class MakeNotificationReadHandler(IUnitOfWork unitOfWork) : IRequestHandler<MakeNotificationReadCommand, int>
    {
        public async Task<int> Handle(MakeNotificationReadCommand request, CancellationToken cancellationToken)
        {
            Notification notification = await unitOfWork.Notifications.FindAsync(n => n.Id == request.NotificationId, cancellationToken) ?? throw new NotFoundException("Notification not found.");
            notification.IsRead = true;
            unitOfWork.Notifications.Update(notification);
            return await unitOfWork.CompleteAsync(cancellationToken);
        }
    }
}
