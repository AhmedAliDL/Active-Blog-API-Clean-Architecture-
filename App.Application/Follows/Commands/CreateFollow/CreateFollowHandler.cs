using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Follows.Commands.CreateFollow
{
    public class CreateFollowHandler(IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ICurrentUserService currentUserService, ILogger<CreateFollowHandler> logger) : IRequestHandler<CreateFollowCommand>
    {
        public Task Handle(CreateFollowCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create follow operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string notInCacheKey = "pending-follow-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            if (usersNotInCache is not null && usersNotInCache.TryGetValue(request.BloggerId, out var delUserIds))
            {
                delUserIds.Remove(userId!.Value);
                memoService.SetObject(notInCacheKey, usersNotInCache);
            }
            string key = $"blog-follows";
            var folllows = memoService.GetObject(key);
            folllows ??= [];

            if (!folllows.TryGetValue(request.BloggerId, out var userIds))
            {
                userIds = [];
                folllows[request.BloggerId] = userIds;
            }

            userIds.Add(userId!.Value);

            memoService.SetObject(key, folllows);
            logger.LogInformation("Create follow operation completed.");
            return Task.CompletedTask;
        }
    }
}
