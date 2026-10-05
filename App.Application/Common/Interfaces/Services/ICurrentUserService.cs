namespace App.Application.Common.Interfaces.Services
{
    public interface ICurrentUserService : IScopedServiceMarker
    {
        Guid? UserId { get; }
    }
}