using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Follows.Commands.DeleteFollow
{
    public class DeleteFollowHandler(IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ICurrentUserService currentUserService, ILogger<DeleteFollowHandler> logger) : IRequestHandler<DeleteFollowCommand>
    {
        public Task Handle(DeleteFollowCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete follow operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string key = $"blog-follows";
            var follows = memoService.GetObject(key);
            if (follows is not null && follows.TryGetValue(request.BloggerId, out var userIds))
            {
                bool res = userIds.Remove(userId!.Value);
                if (res)
                {
                    memoService.SetObject(key, follows);
                    logger.LogInformation("Delete follow operation completed.");
                    return Task.CompletedTask;
                }
            }

            string notInCacheKey = "pending-follow-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            usersNotInCache ??= [];
            if (usersNotInCache.TryGetValue(request.BloggerId, out userIds))
                userIds.Add(userId!.Value);
            else
            {
                usersNotInCache.Add(request.BloggerId, [userId!.Value]);
            }
            memoService.SetObject(notInCacheKey, usersNotInCache);
            logger.LogInformation("Delete follow operation completed.");
            return Task.CompletedTask;
        }
    }
}
