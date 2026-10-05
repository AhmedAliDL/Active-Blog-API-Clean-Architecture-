using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Bookmarks
{
    public class SaveBookmarksToDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SaveBookmarksToDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                logger.LogInformation("Save bookmarks background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"blog-bookmarks";
                var bookmarks = _memoryService.GetObject(key);
                if (bookmarks is not null && bookmarks.Count > 0)
                {
                    List<Bookmark> bookmarksList = [];
                    foreach (var (blogId, userIds) in bookmarks)
                    {
                        foreach (var userId in userIds)
                        {
                            bookmarksList.Add(new Bookmark
                            {
                                BlogId = blogId,
                                UserId = userId
                            });
                        }
                    }
                    await unitOfWork.Bookmarks.AddRangeAsync(bookmarksList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                    logger.LogInformation("Save bookmarks background service operation completed.");
                }

            }
        }
    }
}
