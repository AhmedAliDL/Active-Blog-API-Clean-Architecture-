using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Likes
{
    public class SaveLikesToDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SaveLikesToDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                logger.LogInformation("Save likes background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"blog-likes";
                var likes = _memoryService.GetObject(key);
                if (likes is not null && likes.Count > 0)
                {
                    List<Like> likesList = [];
                    foreach (var (blogId, userIds) in likes)
                    {
                        foreach (var userId in userIds)
                        {
                            likesList.Add(new Like
                            {
                                BlogId = blogId,
                                UserId = userId
                            });
                        }
                    }
                    await unitOfWork.Likes.AddRangeAsync(likesList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                }
                logger.LogInformation("Save likes background service operation completed.");

            }
        }
    }
}
