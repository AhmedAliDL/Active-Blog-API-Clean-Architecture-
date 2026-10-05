using App.Application.Blogs.Commands.CreateBlog;
using App.Application.Blogs.Commands.DeleteBlog;
using App.Application.Blogs.Commands.UpdateBlog;
using App.Application.Blogs.Dto;
using App.Application.Blogs.HttpRequests;
using App.Application.Blogs.Queries.GetAllBlogs;
using App.Application.Blogs.Queries.GetBlogById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles blog CRUD operations.
    /// </summary>
    [Route("api/blogs")]
    [Authorize]
    public class BlogController(IMediator mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all blogs.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of blogs, or no content when no blogs exist.</returns>
        [ProducesResponseType(typeof(List<BlogDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {

            var blogs = await _mediator.Send(new GetAllBlogsQuery(), cancellationToken);
            if (blogs != null && blogs.Count > 0)
                return Ok(blogs);
            return NoContent();

        }
        /// <summary>
        /// Gets a blog by its unique identifier.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The blog details, or no content when the blog is not found.</returns>
        [ProducesResponseType(typeof(BlogDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
        {

            var blog = await _mediator.Send(new GetBlogByIdQuery(id), cancellationToken);
            if (blog != null)
                return Ok(blog);
            return NoContent();

        }
        /// <summary>
        /// Creates a new blog.
        /// </summary>
        /// <param name="req">The blog creation details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost()]
        public async Task<IActionResult> AddBlog([FromBody] CreateBlogCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result > 0)
                return Ok("Add Blog Successfully");
            return BadRequest("Can`t add blog right now try again later.");


        }
        /// <summary>
        /// Updates an existing blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The updated blog details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditBlog(Guid id, [FromBody] UpdateBlogRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new UpdateBlogCommand
            {
                BlogId = id,
                CategoryId = req.CategoryId,
                ImagePath = req.ImagePath,
                Title = req.Title,
            }, cancellationToken);
            if (result > 0)
                return Ok("Blog Edited Successfully!");
            return BadRequest("Can`t edit blog right now try again later.");

        }

        /// <summary>
        /// Deletes a blog by its unique identifier.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteBlog(Guid id, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new DeleteBlogCommand(id), cancellationToken);
            if (result > 0)
                return Ok("Blog deleted successfuly!");
            return BadRequest("Can`t delete blog right now try again later.");

        }


    }
}
