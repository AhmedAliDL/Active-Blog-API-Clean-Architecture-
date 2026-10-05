using App.Application.Notifications.Command.MakeNotificationRead;
using App.Application.Notifications.Command.NotifyAdminMail;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    public class NotificationValidatorTest
    {
        private MakeNotificaitonReadValidator _makeNotificationReadValidator;
        private NotifyAdminMailValidator _notifyAdminMailValidator;

        [SetUp]
        public void Setup()
        {
            _makeNotificationReadValidator =
                new MakeNotificaitonReadValidator();

            _notifyAdminMailValidator =
                new NotifyAdminMailValidator();
        }

        // MakeNotificationReadValidator

        [Test]
        [Category("Notification")]
        public async Task EnsureThatNotificationIdIsValid()
        {
            var command = new MakeNotificationReadCommand(
                Guid.NewGuid());

            var result =
                await _makeNotificationReadValidator.TestValidateAsync(command);

            result.ShouldNotHaveValidationErrorFor(
                n => n.NotificationId);
        }

        [Test]
        [Category("Notification")]
        public async Task EnsureThatNotificationIdIsRequired()
        {
            var command = new MakeNotificationReadCommand(
                Guid.Empty);

            var result =
                await _makeNotificationReadValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(
                n => n.NotificationId);
        }

        // NotifyAdminMailValidator

        [Test]
        [Category("Notification")]
        public async Task EnsureThatNotifyAdminMailIsValid()
        {
            var command = new NotifyAdminMailCommand
            {
                Title = "Contact",
                Message = "Hello Admin"
            };

            var result =
                await _notifyAdminMailValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Notification")]
        public async Task EnsureThatTitleMustBeAtLeastThreeCharacters()
        {
            var command = new NotifyAdminMailCommand
            {
                Title = "Hi",
                Message = "Hello Admin"
            };

            var result =
                await _notifyAdminMailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(
                n => n.Title);
        }

        [Test]
        [Category("Notification")]
        public async Task EnsureThatTitleMustNotExceedTwentyCharacters()
        {
            var command = new NotifyAdminMailCommand
            {
                Title = new string('A', 21),
                Message = "Hello Admin"
            };

            var result =
                await _notifyAdminMailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(
                n => n.Title);
        }

        [Test]
        [Category("Notification")]
        public async Task EnsureThatMessageMustBeAtLeastThreeCharacters()
        {
            var command = new NotifyAdminMailCommand
            {
                Title = "Contact",
                Message = "Hi"
            };

            var result =
                await _notifyAdminMailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(
                n => n.Message);
        }

        [Test]
        [Category("Notification")]
        public async Task EnsureThatMessageMustNotExceedThreeHundredCharacters()
        {
            var command = new NotifyAdminMailCommand
            {
                Title = "Contact",
                Message = new string('A', 301)
            };

            var result =
                await _notifyAdminMailValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(
                n => n.Message);
        }
    }

}
