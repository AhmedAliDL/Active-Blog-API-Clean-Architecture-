using App.Application.Categories.Commands.CreateCategory;
using App.Application.Categories.Commands.DeleteCategory;
using App.Application.Categories.Commands.UpdateCategory;
using App.Application.Categories.Dto;
using App.Application.Categories.HttpRequests;
using App.Application.Categories.Queries.GetAllCategories;
using App.Application.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles category management.
    /// </summary>
    [Route("api/categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController(ISender mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of categories, or no content when no categories exist.</returns>
        [ProducesResponseType(typeof(List<CategoryDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {

            var categories = await _mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
            if (categories != null && categories.Count > 0)
                return Ok(categories);
            return NoContent();

        }
        /// <summary>
        /// Gets a category by its unique identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The category details, or no content when the category is not found.</returns>
        [ProducesResponseType(typeof(CategoryDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
        {

            var category = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
            if (category != null)
                return Ok(category);
            return NoContent();

        }
        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="req">The category creation details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost()]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result > 0)
                return Ok("Add Category Successfully");
            return BadRequest("Can`t add category right now try again later.");


        }
        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="req">The updated category details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCategory(Guid id, [FromBody] UpdateCategoryRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new UpdateCategoryCommand(id, req.CategoryName), cancellationToken);
            if (result > 0)
                return Ok("Category Edited Successfully!");
            return BadRequest("Can`t edit category right now try again later.");
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The category identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            if (result > 0)
                return Ok("Category deleted successfuly!");
            return BadRequest("Can`t delete category right now try again later.");
        }


    }
}
