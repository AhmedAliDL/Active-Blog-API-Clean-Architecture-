using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Notifications.Command.MakeNotificationRead;
using App.Application.Notifications.Command.NotifyAdminMail;
using App.Application.Notifications.Dto;
using App.Application.Notifications.Queries.GetUserNotifications;
using App.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    [Category("Notification")]
    public class NotificationHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<ICurrentUserService> _currentUserService;
        private Mock<IContactService> _contactService;
        private Mock<IIdentityService> _identityService;
        private Mock<IConfiguration> _configuration;
        private Mock<ILogger<NotifyAdminMailHandler>> _logger;

        private MakeNotificationReadHandler _makeNotificationReadHandler;
        private GetUserNotificationsHandler _getUserNotificationsHandler;
        private NotifyAdminMailHandler _notifyAdminMailHandler;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _currentUserService = new Mock<ICurrentUserService>();
            _contactService = new Mock<IContactService>();
            _identityService = new Mock<IIdentityService>();
            _configuration = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<NotifyAdminMailHandler>>();

            _makeNotificationReadHandler =
                new MakeNotificationReadHandler(
                    _unitOfWork.Object);

            _getUserNotificationsHandler =
                new GetUserNotificationsHandler(
                    _unitOfWork.Object,
                    _currentUserService.Object);

            _notifyAdminMailHandler =
                new NotifyAdminMailHandler(
                    _contactService.Object,
                    _currentUserService.Object,
                    _identityService.Object,
                    _configuration.Object,
                    _logger.Object);
        }

        // =========================
        // MakeNotificationRead
        // =========================

        [Test]
        public async Task EnsureThatMakeNotificationReadReturnsOneWhenNotificationIsUpdated()
        {
            var notificationId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notificationId,
                IsRead = false
            };

            _unitOfWork
                .Setup(u => u.Notifications.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(notification);

            _unitOfWork
                .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new MakeNotificationReadCommand(notificationId);

            var result = await _makeNotificationReadHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(notification.IsRead, Is.True);
        }

        [Test]
        public async Task EnsureThatMakeNotificationReadThrowsNotFoundWhenNotificationDoesNotExist()
        {
            _unitOfWork
                .Setup(u => u.Notifications.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Notification?)null);

            var command = new MakeNotificationReadCommand(
                Guid.NewGuid());

            await Assert.ThatAsync(
                async () => await _makeNotificationReadHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task EnsureThatMakeNotificationReadCallsUpdateWhenNotificationExists()
        {
            var notificationId = Guid.NewGuid();

            var notification = new Notification
            {
                Id = notificationId,
                IsRead = false
            };

            _unitOfWork
                .Setup(u => u.Notifications.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(notification);

            _unitOfWork
                .Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _makeNotificationReadHandler.Handle(
                new MakeNotificationReadCommand(notificationId),
                CancellationToken.None);

            _unitOfWork.Verify(
                n => n.Notifications.Update(notification),
                Times.Once);
        }

        // =========================
        // GetUserNotifications
        // =========================

        [Test]
        public async Task EnsureThatGetUserNotificationsReturnsNotificationsWhenUserExists()
        {
            var userId = Guid.NewGuid();

            var notifications = new List<Notification>
            {
                new Notification
                {
                    Id = Guid.NewGuid(),
                    ReceiverId = userId,
                    Message = "New notification",
                    Link = "/blogs/1"
                }
            };

            _currentUserService
                .Setup(u => u.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(u => u.Notifications.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(notifications);

            var result = await _getUserNotificationsHandler.Handle(
                new GetUserNotificationsQuery(),
                CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(1));

            Assert.That(
                result[0].NotificationId,
                Is.EqualTo(notifications[0].Id));

            Assert.That(
                result[0].Message,
                Is.EqualTo("New notification"));
        }

        [Test]
        public async Task EnsureThatGetUserNotificationsReturnsEmptyListWhenThereAreNoNotifications()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(u => u.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(u => u.Notifications.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Notification, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Notification>());

            var result = await _getUserNotificationsHandler.Handle(
                new GetUserNotificationsQuery(),
                CancellationToken.None);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task EnsureThatGetUserNotificationsThrowsNotFoundWhenUserIdIsNull()
        {
            _currentUserService
                .Setup(u => u.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _getUserNotificationsHandler.Handle(
                    new GetUserNotificationsQuery(),
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public async Task EnsureThatGetUserNotificationsThrowsNotFoundWhenUserIdIsEmpty()
        {
            _currentUserService
                .Setup(u => u.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _getUserNotificationsHandler.Handle(
                    new GetUserNotificationsQuery(),
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        // =========================
        // NotifyAdminMail
        // =========================

        [Test]
        public async Task EnsureThatNotifyAdminMailReturnsSuccessWhenEmailIsSent()
        {
            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Email = "user@test.com",
                FName = "Ahmed",
                LName = "Ali"
            };

            var expectedResult = new NotifyDto
            {
                Success = true
            };

            _currentUserService
                .Setup(u => u.UserId)
                .Returns(userId);

            _identityService
                .Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _configuration
                .Setup(c => c["SmtpSettings:AdminEmail"])
                .Returns("admin@test.com");

            _contactService
                .Setup(c => c.SendEmailToAdminServiceAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(expectedResult);

            var command = new NotifyAdminMailCommand
            {
                Title = "Contact",
                Message = "Hello Admin"
            };

            var result = await _notifyAdminMailHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
        }

        [Test]
        public async Task EnsureThatNotifyAdminMailThrowsNotFoundWhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(u => u.UserId)
                .Returns(userId);

            _identityService
                .Setup(i => i.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var command = new NotifyAdminMailCommand
            {
                Title = "Contact",
                Message = "Hello Admin"
            };

            await Assert.ThatAsync(
                async () => await _notifyAdminMailHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
    }
}