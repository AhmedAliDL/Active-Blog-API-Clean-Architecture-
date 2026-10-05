using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Likes.Commands.CreateLike
{
    public class CreateLikeHandler(ICurrentUserService currentUserService, IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ILogger<CreateLikeHandler> logger) : IRequestHandler<CreateLikeCommand>
    {
        public Task Handle(CreateLikeCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create like operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string notInCacheKey = "pending-like-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            if (usersNotInCache is not null && usersNotInCache.TryGetValue(request.BlogId, out var delUserIds))
            {
                delUserIds.Remove(userId!.Value);
                memoService.SetObject(notInCacheKey, usersNotInCache);
            }
            string key = $"blog-likes";
            var likes = memoService.GetObject(key);
            likes ??= [];

            if (!likes.TryGetValue(request.BlogId, out var userIds))
            {
                userIds = [];
                likes[request.BlogId] = userIds;
            }

            userIds.Add(userId!.Value);

            memoService.SetObject(key, likes);
            logger.LogInformation("Create like operation completed.");
            return Task.CompletedTask;
        }
    }
}
