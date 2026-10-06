using App.Application.Bookmarks.Commands.CreateBookmark;
using App.Application.Bookmarks.Commands.DeleteBookmark;
using App.Application.Bookmarks.Dto;
using App.Application.Bookmarks.Queries.GetAllBookmarksOfUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles the blog bookmarks of the current user.
    /// </summary>
    [Authorize]
    public class BookmarkController(ISender mediator) : BaseController(mediator)
    {

        /// <summary>
        /// Gets all blogs bookmarked by the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of bookmarked blogs, or no content when no bookmarks exist.</returns>
        [ProducesResponseType(typeof(List<BookmarkDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("user/bookmarks")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var blogs = await _mediator.Send(new GetAllBookmarksOfUserQuery(), cancellationToken);
            if (blogs != null && blogs.Count > 0)
                return Ok(blogs);
            return NoContent();
        }
        /// <summary>
        /// Bookmarks a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the bookmark is created.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("blogs/{id:guid}/bookmark")]
        public async Task<IActionResult> AddBookmark(Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CreateBookmarkCommand(id), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Removes a blog from the current user's bookmarks.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the bookmark is removed.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("blogs/{id:guid}/bookmark")]
        public async Task<IActionResult> DeleteBookmark(Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteBookmarkCommand(id), cancellationToken);
            return NoContent();

        }


    }
}
