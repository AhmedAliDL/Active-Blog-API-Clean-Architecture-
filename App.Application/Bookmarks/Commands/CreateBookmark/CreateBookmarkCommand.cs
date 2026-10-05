using MediatR;

namespace App.Application.Bookmarks.Commands.CreateBookmark
{
    public record CreateBookmarkCommand(Guid BlogId) : IRequest;
}
