using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.BackgroundServices.Follows
{
    public class DeleteFollowsFromDatabaseBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<DeleteFollowsFromDatabaseBackgroundService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(10), stoppingToken);
                logger.LogInformation("Delete follows background service operation started.");
                using var scope = serviceScopeFactory.CreateScope();
                var _memoryService = scope.ServiceProvider.GetRequiredService<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                string key = $"pending-follow-deletions";
                var follows = _memoryService.GetObject(key);
                if (follows is not null && follows.Count > 0)
                {
                    List<Follow> folowsList = [];
                    foreach (var (bloggerId, userIds) in follows)
                    {
                        folowsList.AddRange(await unitOfWork.Follows.FindAllAsync(l => l.BloggerId == bloggerId && userIds.Contains(l.FollowerId)));
                    }
                    unitOfWork.Follows.DeleteRange(folowsList);
                    await unitOfWork.CompleteAsync();
                    _memoryService.RemoveObject(key);
                    logger.LogInformation("Delete follows background service operation completed.");
                }

            }
        }
    }
}
