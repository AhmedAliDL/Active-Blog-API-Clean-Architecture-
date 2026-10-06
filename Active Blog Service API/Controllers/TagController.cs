using App.Application.Tags.Commands.CreateTag;
using App.Application.Tags.Commands.DeleteTag;
using App.Application.Tags.Commands.UpdateTag;
using App.Application.Tags.Dto;
using App.Application.Tags.HttpRequests;
using App.Application.Tags.Queries.GetAllTagsOfCategory;
using App.Application.Tags.Queries.GetTagById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles tag management.
    /// </summary>
    [Route("api/tags")]
    [Authorize(Roles = "admin")]
    public class TagController(ISender mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all tags of a category.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of tags, or no content when no tags exist.</returns>
        [ProducesResponseType(typeof(List<TagDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet("category/{id:guid}")]
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken = default)
        {

            var tags = await _mediator.Send(new GetAllTagsOfCategoryQuery(id), cancellationToken);
            if (tags != null && tags.Count > 0)
                return Ok(tags);
            return NoContent();


        }
        /// <summary>
        /// Gets a tag by its unique identifier.
        /// </summary>
        /// <param name="id">The tag identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The tag details, or no content when the tag is not found.</returns>
        [ProducesResponseType(typeof(TagDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
        {

            var tag = await _mediator.Send(new GetTagByIdQuery(id), cancellationToken);
            if (tag != null)
                return Ok(tag);
            return NoContent();

        }
        /// <summary>
        /// Creates a new tag in a category.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="req">The tag creation details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("category/{id:guid}")]
        public async Task<IActionResult> AddTag(Guid id, [FromBody] CreateTagRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new CreateTagCommand(id, req.TagName), cancellationToken);
            if (result > 0)
                return Ok("Add Tag Successfully");
            return BadRequest("Can`t add tag right now try again later.");


        }
        /// <summary>
        /// Updates an existing tag.
        /// </summary>
        /// <param name="id">The tag identifier.</param>
        /// <param name="req">The updated tag details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditTag(Guid id, [FromBody] UpdateTagRequest req, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new UpdateTagCommand(id, req.CategoryId, req.TagName), cancellationToken);
            if (result > 0)
                return Ok("Tag Edited Successfully");
            return BadRequest("Can`t edit tag right now try again later.");


        }

        /// <summary>
        /// Deletes a tag by its unique identifier.
        /// </summary>
        /// <param name="id">The tag identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTag(Guid id, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new DeleteTagCommand(id), cancellationToken);
            if (result > 0)
                return Ok("Tag deleted successfuly!");
            return BadRequest("Can`t delete tag right now try again later.");


        }

    }
}
