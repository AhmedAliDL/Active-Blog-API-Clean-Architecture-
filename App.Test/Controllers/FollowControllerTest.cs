using Active_Blog_Service_API.Controllers;
using App.Application.Follows.Commands.CreateFollow;
using App.Application.Follows.Commands.DeleteFollow;
using App.Application.Follows.Dto;
using App.Application.Follows.Queries.GetAllFollowers;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class FollowControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private FollowController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new FollowController(_mediator.Object);
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatGetAllFollowersReturnsOkResultWhenFollowersExist()
        {
            var followers = new List<UserDto>
            {
                new()
                {
                    UserName = "Ahmed Ali",
                    UserImage = "ahmed.jpg"
                },
                new()
                {
                    UserName = "Sayed Ali",
                    UserImage = "sayed.jpg"
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllFollowersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(followers);

            var result = await _controller.GetAllFollowers();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatGetAllFollowersReturnsNoContentWhenNoFollowersExist()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllFollowersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.GetAllFollowers();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatGetAllFollowersReturnsNoContentWhenFollowersAreNull()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllFollowersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<UserDto>)null!);

            var result = await _controller.GetAllFollowers();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatGetAllBloggersReturnsOkResultWhenBloggersExist()
        {
            var bloggers = new List<UserDto>
            {
                new()
                {
                    UserName = "Ahmed Ali",
                    UserImage = "ahmed.jpg"
                },
                new()
                {
                    UserName = "Mohamed Ali",
                    UserImage = "mohamed.jpg"
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllFollowersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bloggers);

            var result = await _controller.GetAllBloggers();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatGetAllBloggersReturnsNoContentWhenNoBloggersExist()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllFollowersQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.GetAllBloggers();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatAddFollowReturnsNoContent()
        {
            var bloggerId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateFollowCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddFollow(
                bloggerId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureThatDeleteFollowReturnsNoContent()
        {
            var bloggerId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteFollowCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteFollow(
                bloggerId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }

}

