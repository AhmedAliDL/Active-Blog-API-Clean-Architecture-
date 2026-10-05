using App.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace App.Infrastructure.Services
{
    public class MemoryService<T>(IMemoryCache _memoryCache) : IMemoryService<T>
    {

        public T GetObject(string key)
        {
            _memoryCache.TryGetValue<T>(key, out var value);

            return value!;
        }
        public T SetObject(string key, T value)
        {
            return _memoryCache.Set(key, value);
        }
        public void RemoveObject(string key)
        {
            _memoryCache.Remove(key);
        }
    }

}