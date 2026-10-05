using App.Application.Auth.Command.ChangePassword;
using App.Application.Auth.Command.ConfirmEmail;
using App.Application.Auth.Command.EditProfile;
using App.Application.Auth.Command.ForgetPassword;
using App.Application.Auth.Command.Login;
using App.Application.Auth.Command.Logout;
using App.Application.Auth.Command.RefreshToken;
using App.Application.Auth.Command.Register;
using App.Application.Auth.Command.ResetPassword;
using App.Application.Auth.Command.SendEmailConfirmation;
using App.Application.Auth.Dto;
using App.Application.Auth.Queries.GetProfile;
using App.Application.Notifications.Dto;
using App.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{
    /// <summary>
    /// Handles user authentication and account management.
    /// </summary>
    [Route("api/auth")]
    public class AuthController(IMediator mediator) : BaseController(mediator)
    {
        private const string RefreshTokenKey = "RefreshToken";
        /// <summary>
        /// Gets the profile of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The profile of the current user, or no content when it is not found.</returns>
        [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile(CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new GetProfileQuery(), cancellationToken);
            if (result != null)
                return Ok(result);
            return NoContent();

        }
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="req">The registration details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The unique identifier of the created user, or an error message when registration fails.</returns>
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand req, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(req, cancellationToken);
            if (result != Guid.Empty)
                return Ok(result);
            return BadRequest("Register process is invalid.");

        }
        /// <summary>
        /// Authenticates a user and returns access and refresh tokens.
        /// </summary>
        /// <param name="req">The login credentials.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The authentication tokens, or an error message when the credentials are invalid.</returns>
        [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand req, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(req, cancellationToken);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
                return Ok(result);
            }
            return BadRequest("Login process is invalid.");

        }
        /// <summary>
        /// Updates the profile of the currently authenticated user.
        /// </summary>
        /// <param name="req">The updated profile details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> EditUser([FromBody] EditProfileCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result.Succeeded)
                return Ok("User Updated successfuly.");
            return BadRequest("User updated failled.");

        }
        /// <summary>
        /// Sends an email confirmation token to the user's email address.
        /// </summary>
        /// <param name="req">The email confirmation request.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or the failure details.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotifyDto), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmaton([FromBody] SendEmailConfirmationCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result.Success)
                return Ok("Confirmation Token sent to your email.");
            return BadRequest(result);

        }
        /// <summary>
        /// Confirms a user's email address with the confirmation token.
        /// </summary>
        /// <param name="req">The email confirmation details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or the failure details.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityResult), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result.Succeeded)
                return Ok("Email confirmed successfully.");
            return BadRequest(result);

        }
        /// <summary>
        /// Changes the password of the currently authenticated user.
        /// </summary>
        /// <param name="req">The current and new password.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or the failure result.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result)
                return Ok("Password changed sucessfuly.");
            return BadRequest(result);

        }
        /// <summary>
        /// Sends a password reset token to the user's email address.
        /// </summary>
        /// <param name="req">The forgot-password request containing the user's email.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or the failure details.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotifyDto), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result.Success)
                return Ok("Confirmation Token sent to your email.");
            return BadRequest(result);

        }
        /// <summary>
        /// Resets the user's password using the reset token.
        /// </summary>
        /// <param name="req">The reset-password details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or the failure result.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(req, cancellationToken);
            if (result)
                return Ok("password has been reseted.");
            return BadRequest(result);

        }

        /// <summary>
        /// Refreshes the access token using the refresh token cookie.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The refreshed tokens, or the failure result.</returns>
        [ProducesResponseType(typeof(ResponseResult<RefreshTokenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseResult<RefreshTokenDto>), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new RefreshTokenCommand(Request.Cookies[RefreshTokenKey]!), cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);

        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expires.ToLocalTime(),
            };
            Response.Cookies.Append(RefreshTokenKey, refreshToken, cookieOptions);

        }
        /// <summary>
        /// Logs out the current user and revokes their refresh token.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>No content when logged out, or the failure result.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseResult<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
        {
            var key = RefreshTokenKey;

            if (!Request.Cookies.TryGetValue(key, out var refreshToken))
                return NoContent();

            var result = await _mediator.Send(
                new LogoutCommand(refreshToken!), cancellationToken);

            Response.Cookies.Delete(key);

            if (!result.IsSuccess)
                return BadRequest(result);

            return NoContent();

        }
    }
}
