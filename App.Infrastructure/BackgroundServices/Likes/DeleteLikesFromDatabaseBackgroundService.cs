using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Likes
{
    public class DeleteLikesFromDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<DeleteLikesFromDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
                logger.LogInformation("Delete likes background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"pending-like-deletions";
                var likes = _memoryService.GetObject(key);
                if (likes is not null && likes.Count > 0)
                {
                    List<Like> likesList = [];
                    foreach (var (blogId, userIds) in likes)
                    {
                        likesList.AddRange(await unitOfWork.Likes.FindAllAsync(l => l.BlogId == blogId && userIds.Contains(l.UserId)));
                    }
                    unitOfWork.Likes.DeleteRange(likesList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                    logger.LogInformation("Delete likes background service operation completed.");
                }

            }
        }
    }
}
