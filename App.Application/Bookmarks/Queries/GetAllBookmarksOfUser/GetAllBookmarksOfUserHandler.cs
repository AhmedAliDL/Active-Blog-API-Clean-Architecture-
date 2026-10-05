using App.Application.Bookmarks.Dto;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Bookmarks.Queries.GetAllBookmarksOfUser
{
    public class GetAllBookmarksOfUserHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<GetAllBookmarksOfUserHandler> logger) : IRequestHandler<GetAllBookmarksOfUserQuery, List<BookmarkDto>>
    {
        public async Task<List<BookmarkDto>> Handle(GetAllBookmarksOfUserQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all bookmarks of user operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            List<BookmarkDto> result = [.. (await unitOfWork.Bookmarks.FindAllAsync(b => b.UserId == userId, cancellationToken)).Select(bm => new BookmarkDto(bm.BlogId))];
            logger.LogInformation("Get all bookmarks of user operation completed.");
            return result;
        }
    }
}
