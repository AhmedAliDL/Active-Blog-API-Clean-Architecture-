namespace App.Application.Common.Interfaces.Services
{
    public interface INotificationService : IScopedServiceMarker
    {
        Task NotifyUserAsync(Guid reciver, string message, Guid sender, string? link);
    }
}