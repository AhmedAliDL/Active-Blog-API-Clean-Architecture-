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
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Notifications.Dto;
using App.Application.Responses;
using App.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class AuthHandlerTest
    {
        private Mock<IIdentityService> _identityService = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;
        private Mock<IRoleService> _roleService = null!;
        private Mock<IJwtTokenService> _jwtTokenService = null!;
        private Mock<IContactService> _contactService = null!;
        private Mock<IConfiguration> _configuration = null!;

        private Mock<ILogger<ChangePasswordHandler>> _changePasswordLogger = null!;
        private Mock<ILogger<ConfirmEmailHandler>> _confirmEmailLogger = null!;
        private Mock<ILogger<EditProfileHandler>> _editProfileLogger = null!;
        private Mock<ILogger<ForgetPasswordHandler>> _forgetPasswordLogger = null!;
        private Mock<ILogger<LoginHandler>> _loginLogger = null!;
        private Mock<ILogger<LogoutHandler>> _logoutLogger = null!;
        private Mock<ILogger<RefreshTokenHandler>> _refreshTokenLogger = null!;
        private Mock<ILogger<RegisterHandler>> _registerLogger = null!;
        private Mock<ILogger<ResetPasswordHandler>> _resetPasswordLogger = null!;
        private Mock<ILogger<SendEmailConfirmationHandler>> _sendEmailLogger = null!;
        private Mock<ILogger<GetProfileHandler>> _getProfileLogger = null!;

        private ChangePasswordHandler _changePasswordHandler = null!;
        private ConfirmEmailHandler _confirmEmailHandler = null!;
        private EditProfileHandler _editProfileHandler = null!;
        private ForgetPasswordHandler _forgetPasswordHandler = null!;
        private LoginHandler _loginHandler = null!;
        private LogoutHandler _logoutHandler = null!;
        private RefreshTokenHandler _refreshTokenHandler = null!;
        private RegisterHandler _registerHandler = null!;
        private ResetPasswordHandler _resetPasswordHandler = null!;
        private SendEmailConfirmationHandler _sendEmailHandler = null!;
        private GetProfileHandler _getProfileHandler = null!;

        [SetUp]
        public void Setup()
        {
            _identityService = new Mock<IIdentityService>();
            _currentUserService = new Mock<ICurrentUserService>();
            _roleService = new Mock<IRoleService>();
            _jwtTokenService = new Mock<IJwtTokenService>();
            _contactService = new Mock<IContactService>();
            _configuration = new Mock<IConfiguration>();

            _changePasswordLogger = new Mock<ILogger<ChangePasswordHandler>>();
            _confirmEmailLogger = new Mock<ILogger<ConfirmEmailHandler>>();
            _editProfileLogger = new Mock<ILogger<EditProfileHandler>>();
            _forgetPasswordLogger = new Mock<ILogger<ForgetPasswordHandler>>();
            _loginLogger = new Mock<ILogger<LoginHandler>>();
            _logoutLogger = new Mock<ILogger<LogoutHandler>>();
            _refreshTokenLogger = new Mock<ILogger<RefreshTokenHandler>>();
            _registerLogger = new Mock<ILogger<RegisterHandler>>();
            _resetPasswordLogger = new Mock<ILogger<ResetPasswordHandler>>();
            _sendEmailLogger = new Mock<ILogger<SendEmailConfirmationHandler>>();
            _getProfileLogger = new Mock<ILogger<GetProfileHandler>>();

            _changePasswordHandler = new ChangePasswordHandler(
                _identityService.Object,
                _currentUserService.Object,
                _changePasswordLogger.Object);

            _confirmEmailHandler = new ConfirmEmailHandler(
                _identityService.Object,
                _confirmEmailLogger.Object);

            _editProfileHandler = new EditProfileHandler(
                _identityService.Object,
                _currentUserService.Object,
                _editProfileLogger.Object);

            _forgetPasswordHandler = new ForgetPasswordHandler(
                _configuration.Object,
                _contactService.Object,
                _identityService.Object,
                _forgetPasswordLogger.Object);

            _loginHandler = new LoginHandler(
                _identityService.Object,
                _jwtTokenService.Object,
                _loginLogger.Object);

            _logoutHandler = new LogoutHandler(
                _jwtTokenService.Object,
                _logoutLogger.Object);

            _refreshTokenHandler = new RefreshTokenHandler(
                _jwtTokenService.Object,
                _refreshTokenLogger.Object);

            _registerHandler = new RegisterHandler(
                _identityService.Object,
                _roleService.Object,
                _registerLogger.Object);

            _resetPasswordHandler = new ResetPasswordHandler(
                _identityService.Object,
                _currentUserService.Object,
                _resetPasswordLogger.Object);

            _sendEmailHandler = new SendEmailConfirmationHandler(
                _identityService.Object,
                _contactService.Object,
                _configuration.Object,
                _sendEmailLogger.Object);

            _getProfileHandler = new GetProfileHandler(
                _identityService.Object,
                _currentUserService.Object,
                _getProfileLogger.Object);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatChangePasswordHandlerReturnsTrueWhenPasswordIsChanged()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var command = new ChangePasswordCommand
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.ChangeUserPasswordAsync(
                    user,
                    command.OldPassword,
                    command.NewPassword))
                .ReturnsAsync(true);

            var result = await _changePasswordHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.True);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatChangePasswordHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var command = new ChangePasswordCommand
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmNewPassword = "NewPassword123"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                async () => await _changePasswordHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailHandlerReturnsSuccessWhenTokenIsValid()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                EmailConfirmed = false
            };

            var command = new ConfirmEmailCommand(userId, "valid-token");

            var identityResult = IdentityResult.Success;

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.CheckEmailConfirmationTokenAsync(
                    user,
                    command.ConfirmationToken))
                .ReturnsAsync(true);

            _identityService
                .Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(identityResult);

            var result = await _confirmEmailHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(identityResult));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var command = new ConfirmEmailCommand(Guid.NewGuid(), "token");

            _identityService
                .Setup(x => x.GetUserByIdAsync(command.UserId))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                 async () => await _confirmEmailHandler.Handle(
                     command,
                     CancellationToken.None),
                 Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailHandlerThrowsArgumentExceptionWhenTokenIsInvalid()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var command = new ConfirmEmailCommand(userId, "invalid-token");

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.CheckEmailConfirmationTokenAsync(
                    user,
                    command.ConfirmationToken))
                .ReturnsAsync(false);

            await Assert.ThatAsync(
                 async () => await _confirmEmailHandler.Handle(
                     command,
                     CancellationToken.None),
                 Throws.TypeOf<ArgumentException>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileHandlerReturnsSuccessWhenUserExists()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FName = "OldFirst",
                LName = "OldLast",
                Email = "old@gmail.com",
                PhoneNumber = "01012345678"
            };

            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "ahmed@gmail.com",
                Phone = "01112345678"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _editProfileHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var command = new EditProfileCommand
            {
                FName = "Ahmed"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                 async () => await _editProfileHandler.Handle(
                     command,
                     CancellationToken.None),
                 Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileHandlerChangesPasswordWhenBothPasswordsAreProvided()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var command = new EditProfileCommand
            {
                CurrentPassword = "OldPassword",
                NewPassword = "NewPassword"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.ChangeUserPasswordAsync(
                    user,
                    command.CurrentPassword,
                    command.NewPassword))
                .ReturnsAsync(true);

            _identityService
                .Setup(x => x.UpdateUserAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _editProfileHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordHandlerReturnsNotificationWhenUserExists()
        {
            var user = new User
            {
                Email = "user@gmail.com"
            };

            var command = new ForgetPasswordCommand(user.Email);

            var notification = new NotifyDto();

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.GenerateForgetPasswordConfirmationTokenAsync(user))
                .ReturnsAsync("confirmation-token");

            _configuration
                .Setup(x => x["SmtpSettings:AdminEmail"])
                .Returns("admin@gmail.com");

            _contactService
                .Setup(x => x.SendEmailServiceAsync(
                    user.Email!,
                    "admin@gmail.com",
                    "Forget Password Confirmation Code",
                    It.Is<string>(x => x.Contains("confirmation-token")),
                    "Active Blog",
                    "User"))
                .ReturnsAsync(notification);

            var result = await _forgetPasswordHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(notification));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var command = new ForgetPasswordCommand("user@gmail.com");

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                async () => await _forgetPasswordHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutHandlerReturnsResultFromJwtService()
        {
            var command = new LogoutCommand("refresh-token");

            var expectedResult = new ResponseResult<bool>();

            _jwtTokenService
                .Setup(x => x.RevokeTokenAsync(command.RefreshToken))
                .ReturnsAsync(expectedResult);

            var result = await _logoutHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRefreshTokenHandlerReturnsResultFromJwtService()
        {
            var command = new RefreshTokenCommand("refresh-token");

            var expectedResult = new ResponseResult<RefreshTokenDto>();

            _jwtTokenService
                .Setup(x => x.RefreshTokenAsync(command.RefreshToken))
                .ReturnsAsync(expectedResult);

            var result = await _refreshTokenHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterHandlerReturnsUserIdWhenUserIsCreatedAndRoleExists()
        {
            var userId = Guid.NewGuid();

            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "ahmed@gmail.com",
                Phone = "01012345678",
                Address = "Cairo",
                ImagePath = "image.jpg",
                Password = "Password123"
            };

            var identityResult = IdentityResult.Success;

            _identityService
                .Setup(x => x.CreateUserAsync(
                    It.IsAny<User>(),
                    command.Password))
                .Callback<User, string>((user, password) =>
                {
                    user.Id = userId;
                })
                .ReturnsAsync(identityResult);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("user"))
                .ReturnsAsync(true);

            _roleService
                .Setup(x => x.AssignRoleToUserAsync(
                    It.IsAny<User>(),
                    "user"));

            var result = await _registerHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(userId));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterHandlerCreatesRoleWhenRoleDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "ahmed@gmail.com",
                Password = "Password123"
            };

            _identityService
                .Setup(x => x.CreateUserAsync(
                    It.IsAny<User>(),
                    command.Password))
                .Callback<User, string>((user, password) =>
                {
                    user.Id = userId;
                })
                .ReturnsAsync(IdentityResult.Success);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("user"))
                .ReturnsAsync(false);

            _roleService
                .Setup(x => x.CreateRoleAsync(
                    "user",
                    "normal user for the system."))
                .ReturnsAsync(IdentityResult.Success);

            _roleService
                .Setup(x => x.AssignRoleToUserAsync(
                    It.IsAny<User>(),
                    "user"));

            var result = await _registerHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(userId));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterHandlerThrowsInvalidOperationExceptionWhenRoleCreationFails()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Email = "ahmed@gmail.com",
                Password = "Password123"
            };

            _identityService
                .Setup(x => x.CreateUserAsync(
                    It.IsAny<User>(),
                    command.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("user"))
                .ReturnsAsync(false);

            _roleService
                .Setup(x => x.CreateRoleAsync(
                    "user",
                    "normal user for the system."))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Description = "Role creation failed"
                        }));

            await Assert.ThatAsync(
                async () => await _registerHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<InvalidOperationException>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordHandlerReturnsTrueWhenPasswordIsReset()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId
            };

            var command = new ResetPasswordCommand
            {
                ConfirmationToken = "token",
                NewPassword = "NewPassword123"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.ResetUserPasswordAsync(
                    user,
                    command.ConfirmationToken,
                    command.NewPassword))
                .ReturnsAsync(true);

            var result = await _resetPasswordHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.True);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var command = new ResetPasswordCommand
            {
                ConfirmationToken = "token",
                NewPassword = "NewPassword123"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                 async () => await _resetPasswordHandler.Handle(
                     command,
                     CancellationToken.None),
                 Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationHandlerReturnsNotificationWhenUserExists()
        {
            var user = new User
            {
                Email = "user@gmail.com"
            };

            var command = new SendEmailConfirmationCommand(user.Email);

            var notification = new NotifyDto();

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _identityService
                .Setup(x => x.GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync("confirmation-token");

            _configuration
                .Setup(x => x["SmtpSettings:AdminEmail"])
                .Returns("admin@gmail.com");

            _contactService
                .Setup(x => x.SendEmailServiceAsync(
                    user.Email!,
                    "admin@gmail.com",
                    "Email Confirmation Code",
                    It.Is<string>(x => x.Contains("confirmation-token")),
                    "Active Blog",
                    "User"))
                .ReturnsAsync(notification);

            var result = await _sendEmailHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(notification));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var command = new SendEmailConfirmationCommand("user@gmail.com");

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                 async () => await _sendEmailHandler.Handle(
                     command,
                     CancellationToken.None),
                 Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatGetProfileHandlerReturnsProfileWhenUserExists()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FName = "Ahmed",
                LName = "Ali",
                Email = "ahmed@gmail.com",
                PhoneNumber = "01012345678",
                Address = "Cairo",
                Image = "profile.jpg"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            var result = await _getProfileHandler.Handle(
                new GetProfileQuery(),
                CancellationToken.None);

            Assert.That(result!.Email, Is.EqualTo(user.Email));
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatGetProfileHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _identityService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                async () => await _getProfileHandler.Handle(
                    new GetProfileQuery(),
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
    }
}

