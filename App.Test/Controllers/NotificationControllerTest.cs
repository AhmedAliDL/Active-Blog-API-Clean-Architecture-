using Active_Blog_Service_API.Controllers;
using App.Application.Notifications.Command.MakeNotificationRead;
using App.Application.Notifications.Command.NotifyAdminMail;
using App.Application.Notifications.Dto;
using App.Application.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
        [Category("Notification")]
        public class NotificationControllerTest
        {
            private Mock<IMediator> _mediator;
            private NotificationController _controller;

            [SetUp]
            public void Setup()
            {
                _mediator = new Mock<IMediator>();
                _controller = new NotificationController(_mediator.Object);
            }

            [Test]
            public async Task EnsureThatGetUserNotificationsReturnsOkWhenNotificationsExist()
            {
                var notifications = new List<UserNotificationDto>
            {
                new UserNotificationDto
                {
                    NotificationId = Guid.NewGuid(),
                    Message = "New notification",
                    SenderId = Guid.NewGuid(),
                    Link = "/blogs/1"
                }
            };

                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<GetUserNotificationsQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(notifications);

                var result = await _controller.GetUserNotifications();

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            public async Task EnsureThatGetUserNotificationsReturnsNoContentWhenNotificationsAreEmpty()
            {
                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<GetUserNotificationsQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<UserNotificationDto>());

                var result = await _controller.GetUserNotifications();

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            public async Task EnsureThatSendMessageToAdminReturnsOkWhenEmailIsSentSuccessfully()
            {
                var command = new NotifyAdminMailCommand
                {
                    Title = "Contact",
                    Message = "Hello Admin"
                };

                var response = new NotifyDto
                {
                    Success = true
                };

                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<NotifyAdminMailCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var result = await _controller.SendMessageToAdmin(command);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            public async Task EnsureThatSendMessageToAdminReturnsBadRequestWhenEmailFails()
            {
                var command = new NotifyAdminMailCommand
                {
                    Title = "Contact",
                    Message = "Hello Admin"
                };

                var response = new NotifyDto
                {
                    Success = false
                };

                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<NotifyAdminMailCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(response);

                var result = await _controller.SendMessageToAdmin(command);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }

            [Test]
            public async Task EnsureThatMakeNotificationReadReturnsOkWhenNotificationIsUpdated()
            {
                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<MakeNotificationReadCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.MakeNotificationRead(
                    Guid.NewGuid());

                Assert.That(result, Is.TypeOf<OkResult>());
            }

            [Test]
            public async Task EnsureThatMakeNotificationReadReturnsBadRequestWhenNotificationIsNotUpdated()
            {
                _mediator
                    .Setup(n => n.Send(
                        It.IsAny<MakeNotificationReadCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.MakeNotificationRead(
                    Guid.NewGuid());

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }
        }
    
}
