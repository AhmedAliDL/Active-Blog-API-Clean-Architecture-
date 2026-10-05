using App.Application.Notifications.Dto;
using MediatR;

namespace App.Application.Notifications.Command.NotifyAdminMail
{
    public record NotifyAdminMailCommand : IRequest<NotifyDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
