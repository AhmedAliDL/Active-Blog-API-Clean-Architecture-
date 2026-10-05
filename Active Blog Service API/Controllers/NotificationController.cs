using App.Application.Notifications.Command.MakeNotificationRead;
using App.Application.Notifications.Command.NotifyAdminMail;
using App.Application.Notifications.Dto;
using App.Application.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles user notifications and contact messages.
    /// </summary>
    [Authorize]
    public class NotificationController(IMediator mediator) : BaseController(mediator)
    {

        /// <summary>
        /// Gets the notifications of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of notifications, or no content when no notifications exist.</returns>
        [ProducesResponseType(typeof(List<UserNotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("user/notifications")]
        public async Task<IActionResult> GetUserNotifications(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserNotificationsQuery(), cancellationToken);
            if (result != null && result.Count > 0)
                return Ok(result);
            return NoContent();
        }

        /// <summary>
        /// Sends a contact message to the administrators.
        /// </summary>
        /// <param name="req">The message details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The send result, or an error message when sending fails.</returns>
        [ProducesResponseType(typeof(NotifyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("contact")]
        public async Task<IActionResult> SendMessageToAdmin([FromBody] NotifyAdminMailCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            Console.WriteLine(result);
            if (result.Success)
                return Ok(result);
            return BadRequest("Can`t send message right now try again later.");

        }
        /// <summary>
        /// Marks a notification as read.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>An empty response on success, or an error message when the update fails.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPatch("notifications/{id:guid}")]
        public async Task<IActionResult> MakeNotificationRead(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new MakeNotificationReadCommand(id), cancellationToken);
            if (result > 0)
                return Ok();
            return BadRequest("Can`t update notification right now try again later.");
        }

    }
}
