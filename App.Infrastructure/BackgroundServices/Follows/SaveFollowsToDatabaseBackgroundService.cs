using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Follows
{
    public class SaveFollowsToDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SaveFollowsToDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                logger.LogInformation("Save follows background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"blog-follows";
                var follows = _memoryService.GetObject(key);
                if (follows is not null && follows.Count > 0)
                {
                    List<Follow> followsList = [];
                    foreach (var (bloggerId, userIds) in follows)
                    {
                        foreach (var userId in userIds)
                        {
                            followsList.Add(new Follow
                            {
                                BloggerId = bloggerId,
                                FollowerId = userId
                            });
                        }
                    }
                    await unitOfWork.Follows.AddRangeAsync(followsList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                    logger.LogInformation("Save follows background service operation completed.");
                }

            }
        }
    }
}
