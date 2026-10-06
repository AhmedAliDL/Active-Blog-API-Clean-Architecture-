using App.Application.ContentBlocks.Commands.CreateContentBlocks;
using App.Application.ContentBlocks.Commands.DeleteContentBlocks;
using App.Application.ContentBlocks.Commands.EditContentBlocks;
using App.Application.ContentBlocks.Dto;
using App.Application.ContentBlocks.HttpRequests;
using App.Application.ContentBlocks.Queries.GetAllContentBlocks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles the content blocks of a blog.
    /// </summary>
    [Route("api/blogs/{id:guid}/content-blocks")]
    [Authorize]
    public class ContentBlockController(ISender mediator) : BaseController(mediator)
    {

        /// <summary>
        /// Gets all content blocks of a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of content blocks, or no content when no blocks exist.</returns>
        [ProducesResponseType(typeof(List<ContentBlockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet()]
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken = default)
        {


            var contentBlocks = await _mediator.Send(new GetAllContentBlocksOfBlogQuery(id), cancellationToken);
            if (contentBlocks != null && contentBlocks.Count > 0)
                return Ok(contentBlocks);
            return NoContent();

        }
        /// <summary>
        /// Adds content blocks to a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The content blocks to add.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost()]
        public async Task<IActionResult> AddImages(Guid id, [FromBody] CreateContentBlocksRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new CreateContentBlocksCommand(id, req.Data), cancellationToken);
            if (result > 0)
                return Ok("Add content blocks successfully");
            return BadRequest("Can`t add content blocks right now try again later.");


        }
        /// <summary>
        /// Updates the content blocks of a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The updated content blocks.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPatch()]
        public async Task<IActionResult> UpdateImages(Guid id, [FromBody] EditContentBlocksRequest req, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new EditContentBlocksCommand(id, req.Data), cancellationToken);
            if (result > 0)
                return Ok("Update content blocks successfully");
            return BadRequest("Can`t update content blocks right now try again later.");
        }
        /// <summary>
        /// Deletes content blocks from a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The identifiers of the content blocks to delete.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteImages(Guid id, [FromBody] DeleteContentBlocksRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new DeleteContentBlocksCommand(id, req.ContentBlocksIds), cancellationToken);
            if (result > 0)
                return Ok("Content Blocks deleted successfuly.");
            return BadRequest("Can`t deleted content blocks right now try again later.");

        }


    }
}
