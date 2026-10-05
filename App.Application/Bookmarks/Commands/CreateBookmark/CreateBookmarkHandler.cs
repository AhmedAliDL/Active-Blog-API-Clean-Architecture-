using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Bookmarks.Commands.CreateBookmark
{
    public class CreateBookmarkHandler(IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ICurrentUserService currentUserService, ILogger<CreateBookmarkHandler> logger) : IRequestHandler<CreateBookmarkCommand>
    {
        public Task Handle(CreateBookmarkCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create bookmark operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string notInCacheKey = "pending-bookmarks-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            if (usersNotInCache is not null && usersNotInCache.TryGetValue(request.BlogId, out var delUserIds))
            {
                delUserIds.Remove(userId!.Value);
                memoService.SetObject(notInCacheKey, usersNotInCache);
            }
            string key = $"blog-bookmarks";
            var bookmarks = memoService.GetObject(key);
            bookmarks ??= [];

            if (!bookmarks.TryGetValue(request.BlogId, out var userIds))
            {
                userIds = [];
                bookmarks[request.BlogId] = userIds;
            }

            userIds.Add(userId!.Value);

            memoService.SetObject(key, bookmarks);
            logger.LogInformation("Create bookmark operation completed.");
            return Task.CompletedTask;
        }
    }
}
