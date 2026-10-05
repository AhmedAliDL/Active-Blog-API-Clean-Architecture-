using App.Application.Comments.Command.CreateComment;
using App.Application.Comments.Command.DeleteComment;
using App.Application.Comments.Command.UpdateComment;
using App.Application.Comments.Dto;
using App.Application.Comments.HttpRequests;
using App.Application.Comments.Queries.GetAllCommentOfBlog;
using App.Application.Comments.Queries.GetCommentById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{

    /// <summary>
    /// Handles blog comments.
    /// </summary>
    [Authorize]
    public class CommentController(IMediator mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all comments of a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of comments, or no content when no comments exist.</returns>
        [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet("blogs/{id:guid}/comments")]
        public async Task<IActionResult> Index(Guid id, CancellationToken cancellationToken = default)
        {

            var comments = await _mediator.Send(new GetCommentsOfBlogQuery(id), cancellationToken);
            if (comments != null && comments.Count > 0)
                return Ok(comments);
            return NoContent();

        }
        /// <summary>
        /// Gets a comment by its unique identifier.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The comment details, or no content when the comment is not found.</returns>
        [ProducesResponseType(typeof(GetCommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet("comments/{id:guid}")]
        public async Task<IActionResult> GetCommentById(Guid id, CancellationToken cancellationToken = default)
        {

            var comment = await _mediator.Send(new GetCommentByIdQuery(id), cancellationToken);
            if (comment != null)
                return Ok(comment);
            return NoContent();

        }

        /// <summary>
        /// Adds a comment to a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The comment details, including the parent comment for replies.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("blogs/{id:guid}/comments")]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new CreateCommentCommand
            {
                BlogId = id,
                CommentContent = req.CommentContent,
                ParentCommentId = req.ParentCommentId,
            }, cancellationToken);

            if (result > 0)
                return Ok("Comment successfuly added.");
            return BadRequest("Can`t add comment right now try again later.");


        }
        /// <summary>
        /// Updates a comment.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <param name="req">The updated comment details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("blogs/{id:guid}/comments")]
        public async Task<IActionResult> EditComment(Guid id, [FromBody] UpdateCommentRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new UpdateCommentCommand(id, req.CommentContent, req.BlogId), cancellationToken);
            if (result > 0)
                return Ok("Comment successfuly edited.");
            return BadRequest("Can`t edit comment right now try again later.");

        }

        /// <summary>
        /// Deletes a comment.
        /// </summary>
        /// <param name="id">The comment identifier.</param>
        /// <param name="req">The comment deletion details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("blogs/{id:guid}/comments")]
        public async Task<IActionResult> DeleteComment(Guid id, [FromBody] DeleteCommentRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new DeleteCommentCommand(id, req.BlogId), cancellationToken);
            if (result > 0)
                return Ok("Comment successfuly deleted.");
            return BadRequest("Can`t delete comment right now try again later.");

        }
    }
}
