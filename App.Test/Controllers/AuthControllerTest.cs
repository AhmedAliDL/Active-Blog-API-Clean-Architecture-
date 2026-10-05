using Active_Blog_Service_API.Controllers;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class AuthControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private AuthController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new AuthController(_mediator.Object);

            var httpContext = new DefaultHttpContext();

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatProfileReturnsOkWhenProfileExists()
        {

            var profile = new ProfileDto
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "example@gmail.com",

            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<GetProfileQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(profile);

            var result = await _controller.Profile();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatProfileReturnsNoContentWhenProfileDoesNotExist()
        {
            _mediator.Setup(m => m.Send(
                    It.IsAny<GetProfileQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProfileDto?)null);

            var result = await _controller.Profile();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterReturnsOkWhenRegistrationSucceeds()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "example@gmail.com",
                Password = "xx@12gDMaidw2l3",
                ConfirmPassword = "xx@12gDMaidw2l3",

            };

            var userId = Guid.NewGuid();

            _mediator.Setup(m => m.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            var result = await _controller.Register(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterReturnsBadRequestWhenRegistrationFails()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "example@gmail.com",
                Password = "xx@12gDMaidw2l3",
                ConfirmPassword = "xx@12gDMaidw2l3",
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<RegisterCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.Empty);

            var result = await _controller.Register(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLoginReturnsOkWhenLoginSucceeds()
        {
            var command = new LoginCommand
            {
                Email = "example@gmail.com",
                Password = "xx@12gDMaidw2l3"
            };

            var loginResult = new LoginDto
            {
                RefreshToken = "refresh-token",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
                Expiration = DateTime.UtcNow.AddMinutes(30),
                Token = "jwt-token"
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<LoginCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginResult);

            var result = await _controller.Login(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());

            Assert.That(
                _controller.Response.Headers["Set-Cookie"].ToString(),
                Does.Contain("RefreshToken"));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLoginReturnsBadRequestWhenLoginFails()
        {
            var command = new LoginCommand
            {
                Email = "example@gmail.com",
                Password = "xx@12gDMaidw2l3"
            };

            var loginResult = new LoginDto
            {
                RefreshToken = string.Empty,
                RefreshTokenExpiration = DateTime.UtcNow,
                Expiration = DateTime.UtcNow,
                Token = string.Empty,

            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<LoginCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(loginResult);

            var result = await _controller.Login(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditUserReturnsOkWhenProfileIsUpdated()
        {
            var command = new EditProfileCommand
            {
                FName = "Sayed",
                LName = "Ali",

            };

            var response = IdentityResult.Success;

            _mediator.Setup(m => m.Send(
                    It.IsAny<EditProfileCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.EditUser(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditUserReturnsBadRequestWhenProfileIsNotUpdated()
        {
            var command = new EditProfileCommand
            {
                FName = "Sayed",
                LName = "Ali",
            };

            var response = IdentityResult.Failed();

            _mediator.Setup(m => m.Send(
                    It.IsAny<EditProfileCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.EditUser(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationReturnsOkWhenEmailIsSent()
        {
            var command = new SendEmailConfirmationCommand("example@gmail.com");

            var response = new NotifyDto
            {
                Message = "Successed",
                Success = true
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<SendEmailConfirmationCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.SendEmailConfirmaton(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationReturnsBadRequestWhenEmailIsNotSent()
        {
            var command = new SendEmailConfirmationCommand("example@gmail.com");

            var response = new NotifyDto
            {
                Message = "Successed",
                Success = false
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<SendEmailConfirmationCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.SendEmailConfirmaton(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailReturnsOkWhenEmailIsConfirmed()
        {
            var command = new ConfirmEmailCommand(Guid.NewGuid(), "confirmation-token");

            var response = IdentityResult.Success;

            _mediator.Setup(m => m.Send(
                    It.IsAny<ConfirmEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ConfirmEmail(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailReturnsBadRequestWhenEmailConfirmationFails()
        {
            var command = new ConfirmEmailCommand(Guid.NewGuid(), "confirmation-token");

            var response = IdentityResult.Failed();

            _mediator.Setup(m => m.Send(
                    It.IsAny<ConfirmEmailCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ConfirmEmail(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatChangePasswordReturnsOkWhenPasswordIsChanged()
        {
            var command = new ChangePasswordCommand
            {
                OldPassword = "AhmedAi2@Sed",
                NewPassword = "AHmed23D@gd2",
                ConfirmNewPassword = "AHmed23D@gd2"
            };
            _mediator.Setup(m => m.Send(
                    It.IsAny<ChangePasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.ChangePassword(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatChangePasswordReturnsBadRequestWhenPasswordIsNotChanged()
        {
            var command = new ChangePasswordCommand
            {
                OldPassword = "AhmedAi2@Sed",
                NewPassword = "AHmed23D@gd2",
                ConfirmNewPassword = "AHmed23D@gd2"
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<ChangePasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.ChangePassword(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordReturnsOkWhenEmailIsSent()
        {
            var command = new ForgetPasswordCommand("example@gmail.com");

            var response = new NotifyDto
            {
                Message = "Success",
                Success = true
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<ForgetPasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ForgetPassword(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordReturnsBadRequestWhenProcessFails()
        {
            var command = new ForgetPasswordCommand("example@gmail.com");

            var response = new NotifyDto
            {
                Message = "Fail",
                Success = false
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<ForgetPasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ForgetPassword(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordReturnsOkWhenPasswordIsReset()
        {
            var command = new ResetPasswordCommand
            {
                ConfirmationToken = "confirmation-token",
                NewPassword = "Ahmed@12ddFFF",
                ConfirmNewPassword = "Ahmed@12ddFFF"
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<ResetPasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.ResetPassword(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordReturnsBadRequestWhenPasswordIsNotReset()
        {
            var command = new ResetPasswordCommand
            {
                ConfirmationToken = "confirmation-token",
                NewPassword = "Ahmed@12ddFFF",
                ConfirmNewPassword = "Ahmed@12ddFFF"
            };

            _mediator.Setup(m => m.Send(
                    It.IsAny<ResetPasswordCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.ResetPassword(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRefreshTokenReturnsOkWhenRefreshTokenIsValid()
        {
            var refreshToken = "valid-refresh-token";

            _controller.HttpContext.Request.Headers.Cookie =
                $"RefreshToken={refreshToken}";

            var response = new ResponseResult<RefreshTokenDto>
            {
                IsSuccess = true
            };

            _mediator.Setup(m => m.Send(
                    It.Is<RefreshTokenCommand>(
                        x => x.RefreshToken == refreshToken),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.RefreshToken();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRefreshTokenReturnsBadRequestWhenRefreshTokenIsInvalid()
        {
            var refreshToken = "invalid-refresh-token";

            _controller.HttpContext.Request.Headers.Cookie =
                $"RefreshToken={refreshToken}";

            var response = new ResponseResult<RefreshTokenDto>
            {
                IsSuccess = false
            };

            _mediator.Setup(m => m.Send(
                    It.Is<RefreshTokenCommand>(
                        x => x.RefreshToken == refreshToken),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.RefreshToken();

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutReturnsNoContentWhenRefreshTokenCookieDoesNotExist()
        {
            var result = await _controller.Logout();

            Assert.That(result, Is.TypeOf<NoContentResult>());

            _mediator.Verify(
                m => m.Send(
                    It.IsAny<LogoutCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutReturnsNoContentWhenLogoutSucceeds()
        {
            var refreshToken = "refresh-token";

            _controller.HttpContext.Request.Headers.Cookie =
                $"RefreshToken={refreshToken}";

            var response = new ResponseResult<bool>
            {
                IsSuccess = true
            };

            _mediator.Setup(m => m.Send(
                    It.Is<LogoutCommand>(
                        x => x.RefreshToken == refreshToken),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.Logout();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutReturnsBadRequestWhenLogoutFails()
        {
            var refreshToken = "refresh-token";

            _controller.HttpContext.Request.Headers.Cookie =
                $"RefreshToken={refreshToken}";

            var response = new ResponseResult<bool>
            {
                IsSuccess = false
            };

            _mediator.Setup(m => m.Send(
                    It.Is<LogoutCommand>(
                        x => x.RefreshToken == refreshToken),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.Logout();

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }
}
