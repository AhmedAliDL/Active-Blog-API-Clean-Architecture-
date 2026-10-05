using App.Application.Follows.Commands.CreateFollow;
using App.Application.Follows.Commands.DeleteFollow;
using App.Application.Follows.Dto;
using App.Application.Follows.Queries.GetAllFollowers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles follow relationships between users.
    /// </summary>
    [Authorize]
    public class FollowController(IMediator mediator) : BaseController(mediator)
    {

        /// <summary>
        /// Gets the followers of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of followers, or no content when no followers exist.</returns>
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("followers")]
        public async Task<IActionResult> GetAllFollowers(CancellationToken cancellationToken = default)
        {

            var followers = await _mediator.Send(new GetAllFollowersQuery(), cancellationToken);
            if (followers != null && followers.Count > 0)
                return Ok(followers);
            return NoContent();
        }

        /// <summary>
        /// Gets all bloggers.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of bloggers, or no content when no bloggers exist.</returns>
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("bloggers")]
        public async Task<IActionResult> GetAllBloggers(CancellationToken cancellationToken = default)
        {

            var bloggers = await _mediator.Send(new GetAllFollowersQuery(), cancellationToken);
            if (bloggers != null && bloggers.Count > 0)
                return Ok(bloggers);
            return NoContent();

        }

        /// <summary>
        /// Follows a blogger.
        /// </summary>
        /// <param name="id">The blogger identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the follow is created.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("follows/bloggers/{id:guid}")]
        public async Task<IActionResult> AddFollow(Guid id, CancellationToken cancellationToken)
        {

            await _mediator.Send(new CreateFollowCommand(id), cancellationToken = default);
            return NoContent();
        }

        /// <summary>
        /// Unfollows a blogger.
        /// </summary>
        /// <param name="id">The blogger identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when the follow is removed.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("follows/bloggers/{id:guid}")]
        public async Task<IActionResult> DeleteFollow(Guid id, CancellationToken cancellationToken = default)
        {

            await _mediator.Send(new DeleteFollowCommand(id), cancellationToken);
            return NoContent();
        }


    }
}
