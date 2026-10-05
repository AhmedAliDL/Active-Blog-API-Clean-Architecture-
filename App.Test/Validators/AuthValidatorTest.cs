using App.Application.Auth.Command.ConfirmEmail;
using App.Application.Auth.Command.EditProfile;
using App.Application.Auth.Command.ForgetPassword;
using App.Application.Auth.Command.Login;
using App.Application.Auth.Command.Logout;
using App.Application.Auth.Command.RefreshToken;
using App.Application.Auth.Command.Register;
using App.Application.Auth.Command.ResetPassword;
using App.Application.Auth.Command.SendEmailConfirmation;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class AuthValidatorTest
    {
        private ConfirmEmailValidator _confirmEmailValidator = null!;
        private EditProfileValidator _editProfileValidator = null!;
        private ForgetPasswordValidator _forgetPasswordValidator = null!;
        private LoginValidator _loginValidator = null!;
        private LogoutValidator _logoutValidator = null!;
        private RefreshTokenValidator _refreshTokenValidator = null!;
        private RegisterValidator _registerValidator = null!;
        private ResetPasswordValidator _resetPasswordValidator = null!;
        private SendEmailConfirmationValidator _sendEmailConfirmationValidator = null!;

        [SetUp]
        public void Setup()
        {
            _confirmEmailValidator = new ConfirmEmailValidator();
            _editProfileValidator = new EditProfileValidator();
            _forgetPasswordValidator = new ForgetPasswordValidator();
            _loginValidator = new LoginValidator();
            _logoutValidator = new LogoutValidator();
            _refreshTokenValidator = new RefreshTokenValidator();
            _registerValidator = new RegisterValidator();
            _resetPasswordValidator = new ResetPasswordValidator();
            _sendEmailConfirmationValidator = new SendEmailConfirmationValidator();
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailCommandIsValid()
        {
            var command = new ConfirmEmailCommand(Guid.NewGuid(), "valid-token");

            var result = await _confirmEmailValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailCommandIsInvalidWhenUserIdIsEmpty()
        {
            var command = new ConfirmEmailCommand(Guid.Empty, "valid-token");

            var result = await _confirmEmailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatConfirmEmailCommandIsInvalidWhenConfirmationTokenIsEmpty()
        {
            var command = new ConfirmEmailCommand(Guid.NewGuid(), "");

            var result = await _confirmEmailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmationToken);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandIsValid()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandFirstNameIsInvalidWhenTooShort()
        {
            var command = new EditProfileCommand
            {
                FName = "Ab",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.FName);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandFirstNameIsInvalidWhenTooLong()
        {
            var command = new EditProfileCommand
            {
                FName = new string('A', 31),
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.FName);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandLastNameIsInvalidWhenTooShort()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Al",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.LName);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandLastNameIsInvalidWhenTooLong()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = new string('A', 31),
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.LName);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandPhoneIsInvalidWhenFormatIsWrong()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "123456789",
                Email = "ahmed@gmail.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandEmailIsInvalidWhenEmailFormatIsWrong()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "invalid-email"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandEmailIsInvalidWhenDomainIsNotAllowed()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@outlook.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatEditProfileCommandAcceptsAllowedEmailDomain()
        {
            var command = new EditProfileCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@yahoo.com"
            };

            var result = await _editProfileValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordCommandIsValid()
        {
            var command = new ForgetPasswordCommand("ahmed@gmail.com");

            var result = await _forgetPasswordValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordCommandEmailIsInvalidWhenFormatIsWrong()
        {
            var command = new ForgetPasswordCommand("invalid-email");

            var result = await _forgetPasswordValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatForgetPasswordCommandEmailIsInvalidWhenDomainIsNotAllowed()
        {
            var command = new ForgetPasswordCommand("ahmed@outlook.com");

            var result = await _forgetPasswordValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatLoginCommandIsValid()
        {
            var command = new LoginCommand
            {
                Email = "ahmed@gmail.com"
            };

            var result = await _loginValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLoginCommandEmailIsInvalidWhenFormatIsWrong()
        {
            var command = new LoginCommand
            {
                Email = "invalid-email"
            };

            var result = await _loginValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLoginCommandEmailIsInvalidWhenDomainIsNotAllowed()
        {
            var command = new LoginCommand
            {
                Email = "ahmed@outlook.com"
            };

            var result = await _loginValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutCommandIsValid()
        {
            var command = new LogoutCommand("valid-refresh-token");

            var result = await _logoutValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatLogoutCommandIsInvalidWhenRefreshTokenIsEmpty()
        {
            var command = new LogoutCommand("");

            var result = await _logoutValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRefreshTokenCommandIsValid()
        {
            var command = new RefreshTokenCommand("valid-refresh-token");

            var result = await _refreshTokenValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRefreshTokenCommandIsInvalidWhenRefreshTokenIsEmpty()
        {
            var command = new RefreshTokenCommand("");

            var result = await _refreshTokenValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandIsValid()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandFirstNameIsInvalidWhenTooShort()
        {
            var command = new RegisterCommand
            {
                FName = "Ab",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.FName);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandFirstNameIsInvalidWhenTooLong()
        {
            var command = new RegisterCommand
            {
                FName = new string('A', 31),
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.FName);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandLastNameIsInvalidWhenTooShort()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Al",
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.LName);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandLastNameIsInvalidWhenTooLong()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = new string('A', 31),
                Phone = "01012345678",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.LName);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandPhoneIsInvalidWhenFormatIsWrong()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "123456789",
                Email = "ahmed@gmail.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Phone);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandEmailIsInvalidWhenFormatIsWrong()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "invalid-email"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatRegisterCommandEmailIsInvalidWhenDomainIsNotAllowed()
        {
            var command = new RegisterCommand
            {
                FName = "Ahmed",
                LName = "Ali",
                Phone = "01012345678",
                Email = "ahmed@outlook.com"
            };

            var result = await _registerValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationCommandIsValid()
        {
            var command = new SendEmailConfirmationCommand("ahmed@gmail.com");

            var result = await _sendEmailConfirmationValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationCommandEmailIsInvalidWhenFormatIsWrong()
        {
            var command = new SendEmailConfirmationCommand("invalid-email");

            var result = await _sendEmailConfirmationValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatSendEmailConfirmationCommandEmailIsInvalidWhenDomainIsNotAllowed()
        {
            var command = new SendEmailConfirmationCommand("ahmed@outlook.com");

            var result = await _sendEmailConfirmationValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordCommandIsValid()
        {
            var command = new ResetPasswordCommand
            {
                ConfirmationToken = "valid-token"
            };

            var result = await _resetPasswordValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Auth")]
        public async Task EnsureThatResetPasswordCommandIsInvalidWhenConfirmationTokenIsEmpty()
        {
            var command = new ResetPasswordCommand
            {
                ConfirmationToken = ""
            };

            var result = await _resetPasswordValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmationToken);
        }
    }
}

