using App.Application.Bookmarks.Dto;
using MediatR;

namespace App.Application.Bookmarks.Queries.GetAllBookmarksOfUser
{
    public record GetAllBookmarksOfUserQuery : IRequest<List<BookmarkDto>>;
}
