using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Bookmarks
{
    public class DeleteBookmarksFromDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<DeleteBookmarksFromDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
                logger.LogInformation("Delete bookmarks background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"pending-bookmark-deletions";
                var bookmarks = _memoryService.GetObject(key);
                if (bookmarks is not null && bookmarks.Count > 0)
                {
                    List<Bookmark> bookmarksList = [];
                    foreach (var (blogId, userIds) in bookmarks)
                    {
                        bookmarksList.AddRange(await unitOfWork.Bookmarks.FindAllAsync(l => l.BlogId == blogId && userIds.Contains(l.UserId)));
                    }
                    unitOfWork.Bookmarks.DeleteRange(bookmarksList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                    logger.LogInformation("Delete bookmarks background service operation completed.");
                }

            }
        }
    }
}
