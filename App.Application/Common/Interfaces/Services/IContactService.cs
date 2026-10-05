using App.Application.Notifications.Dto;

namespace App.Application.Common.Interfaces.Services
{
    public interface IContactService : IScopedServiceMarker
    {
        Task<NotifyDto> SendEmailServiceAsync(string toEmailAdd, string fromEmailAdd, string subject, string body, string fromName, string toName);
        Task<NotifyDto> SendEmailToAdminServiceAsync(string toEmailAdd, string fromEmailAdd, string subject, string body, string fromName, string toName);
    }
}