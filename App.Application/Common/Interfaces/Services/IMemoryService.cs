namespace App.Application.Common.Interfaces.Services
{
    public interface IMemoryService<T> : IScopedServiceMarker
    {
        T GetObject(string key);
        T SetObject(string key, T value);
        void RemoveObject(string key);
    }
}