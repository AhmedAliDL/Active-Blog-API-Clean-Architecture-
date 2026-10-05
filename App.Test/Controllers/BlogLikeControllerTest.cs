using Active_Blog_Service_API.Controllers;
using App.Application.Likes.Commands.CreateLike;
using App.Application.Likes.Commands.DeleteLike;
using App.Application.Likes.Dto;
using App.Application.Likes.Queries.GetAllBlogLikes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class BlogLikeControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private BlogLikeController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new BlogLikeController(_mediator.Object);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatIndexReturnsOkResultWhenLikesExist()
        {
            var blogId = Guid.NewGuid();

            var likes = new List<LikeDto>
            {
                new()
                {
                    LikeId = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    LikeId = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBlogLikesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(likes);

            var result = await _controller.Index(
                blogId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatIndexReturnsNoContentWhenNoLikesExist()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBlogLikesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.Index(
                blogId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatIndexReturnsNoContentWhenLikesAreNull()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBlogLikesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<LikeDto>)null!);

            var result = await _controller.Index(
                blogId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatAddLikeReturnsNoContentWhenLikeIsAdded()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateLikeCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddLike(
                blogId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatDeleteLikeReturnsNoContentWhenLikeIsDeleted()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteLikeCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteLike(
                blogId,
                CancellationToken.None);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }

}
