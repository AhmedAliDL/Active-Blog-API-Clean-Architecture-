using App.Application.Likes.Commands.CreateLike;
using App.Application.Likes.Commands.DeleteLike;
using App.Application.Likes.Dto;
using App.Application.Likes.Queries.GetAllBlogLikes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles the likes of a blog.
    /// </summary>
    [Route("api/blogs/{id:guid}/likes")]
    [Authorize]
    public class BlogLikeController(ISender mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all likes of a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of likes, or no content when no likes exist.</returns>
        [ProducesResponseType(typeof(List<LikeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "admin")]
        [HttpGet()]
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken = default)
        {
            var likes = await _mediator.Send(new GetAllBlogLikesQuery(id), cancellationToken);
            if (likes == null || likes.Count <= 0)
                return NoContent();
            return Ok(likes);

        }

        /// <summary>
        /// Likes a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the like is created.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost()]
        public async Task<IActionResult> AddLike(Guid id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new CreateLikeCommand(id), cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Removes the current user's like from a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the like is removed.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteLike(Guid id, CancellationToken cancellationToken = default)
        {

            await _mediator.Send(new DeleteLikeCommand(id), cancellationToken);
            return NoContent();
        }


    }
}
