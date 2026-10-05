using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Likes.Commands.DeleteLike
{
    public class DeleteLikeHandler(ICurrentUserService currentUserService, IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ILogger<DeleteLikeHandler> logger) : IRequestHandler<DeleteLikeCommand>
    {
        public Task Handle(DeleteLikeCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete like operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string key = $"blog-likes";
            var likes = memoService.GetObject(key);
            if (likes is not null && likes.TryGetValue(request.BlogId, out var userIds))
            {
                bool res = userIds.Remove(userId!.Value);
                if (res)
                {
                    memoService.SetObject(key, likes);
                    logger.LogInformation("Delete like operation completed.");
                    return Task.CompletedTask;
                }
            }

            string notInCacheKey = "pending-like-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            usersNotInCache ??= [];
            if (usersNotInCache.TryGetValue(request.BlogId, out userIds))
                userIds.Add(userId!.Value);
            else
            {
                usersNotInCache.Add(request.BlogId, [userId!.Value]);
            }
            memoService.SetObject(notInCacheKey, usersNotInCache);
            logger.LogInformation("Delete like operation completed.");
            return Task.CompletedTask;
        }
    }
}
