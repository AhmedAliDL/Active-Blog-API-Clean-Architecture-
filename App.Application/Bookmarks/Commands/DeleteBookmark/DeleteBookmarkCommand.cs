using MediatR;

namespace App.Application.Bookmarks.Commands.DeleteBookmark
{
    public record DeleteBookmarkCommand(Guid BlogId) : IRequest;
}
