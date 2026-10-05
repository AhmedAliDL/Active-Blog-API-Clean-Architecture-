using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Bookmarks.Commands.DeleteBookmark
{
    public class DeleteBookmarkHandler(IMemoryService<Dictionary<Guid, HashSet<Guid>>> memoService, ICurrentUserService currentUserService, ILogger<DeleteBookmarkHandler> logger) : IRequestHandler<DeleteBookmarkCommand>
    {
        public Task Handle(DeleteBookmarkCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete bookmark operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            string key = $"blog-bookmarks";
            var bookmarks = memoService.GetObject(key);
            if (bookmarks is not null && bookmarks.TryGetValue(request.BlogId, out var userIds))
            {
                bool res = userIds.Remove(userId!.Value);
                if (res)
                {
                    memoService.SetObject(key, bookmarks);
                    logger.LogInformation("Delete bookmark operation completed.");
                    return Task.CompletedTask;
                }
            }

            string notInCacheKey = "pending-bookmark-deletions";
            var usersNotInCache = memoService.GetObject(notInCacheKey);
            usersNotInCache ??= [];
            if (usersNotInCache.TryGetValue(request.BlogId, out userIds))
                userIds.Add(userId!.Value);
            else
            {
                usersNotInCache.Add(request.BlogId, [userId!.Value]);
            }
            memoService.SetObject(notInCacheKey, usersNotInCache);
            logger.LogInformation("Delete bookmark operation completed.");
            return Task.CompletedTask;
        }
    }
}
