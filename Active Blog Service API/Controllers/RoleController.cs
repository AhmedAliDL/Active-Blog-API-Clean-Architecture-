using App.Application.Roles.Command.AssignRole;
using App.Application.Roles.Command.CreateRole;
using App.Application.Roles.Command.DeleteAssignRole;
using App.Application.Roles.Command.DeleteRole;
using App.Application.Roles.Dto;
using App.Application.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles role management and role assignment to users.
    /// </summary>
    [Route("api/roles")]
    [Authorize(Roles = "admin")]
    public class RoleController(ISender mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of roles, or no content when no roles exist.</returns>
        [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet()]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {

            var rolesDto = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);
            if (rolesDto == null || rolesDto.Count <= 0)
                return NoContent();
            return Ok(rolesDto);

        }
        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <param name="req">The role creation details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when creation fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost()]
        public async Task<IActionResult> AddRole([FromBody] CreateRoleCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result.Succeeded)
                return Ok("Role added successfuly");
            return BadRequest("Can`t add role right now try again later.");

        }
        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="req">The role assignment details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the assignment fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost("users")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand req, CancellationToken cancellationToken = default)
        {


            var result = await _mediator.Send(req, cancellationToken);
            if (result.Succeeded)
                return Ok("Role assigned successfuly");
            return BadRequest("Can`t assign role right now try again later.");

        }
        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="req">The role deletion details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when deletion fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete()]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleCommand req, CancellationToken cancellationToken = default)
        {


            var result = await _mediator.Send(req, cancellationToken);
            if (result)
                return Ok("Role deleted successfuly");
            return BadRequest("Can`t delete role right now try again later.");

        }
        /// <summary>
        /// Removes a role assignment from a user.
        /// </summary>
        /// <param name="req">The role assignment removal details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when removal fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("users")]
        public async Task<IActionResult> DeleteAssignRole([FromBody] DeleteAssignRoleCommand req, CancellationToken cancellationToken = default)
        {


            var result = await _mediator.Send(req, cancellationToken);
            if (result)
                return Ok("Assign role deleted successfuly");
            return BadRequest("Can`t delete assigned role right now try again later.");

        }
    }
}
