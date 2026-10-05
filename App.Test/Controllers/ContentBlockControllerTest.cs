using Active_Blog_Service_API.Controllers;
using App.Application.ContentBlocks.Commands.CreateContentBlocks;
using App.Application.ContentBlocks.Commands.DeleteContentBlocks;
using App.Application.ContentBlocks.Commands.EditContentBlocks;
using App.Application.ContentBlocks.Dto;
using App.Application.ContentBlocks.HttpRequests;
using App.Application.ContentBlocks.Queries.GetAllContentBlocks;
using App.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class ContentBlockControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private ContentBlockController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new ContentBlockController(_mediator.Object);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatIndexReturnsOkResultWhenContentBlocksExist()
        {
            var blogId = Guid.NewGuid();

            var contentBlocks = new List<ContentBlockDto>
            {
                new()
                {
                    BlockId = Guid.NewGuid(),
                    Type = ContentBlockType.Text,
                    Content = "Test content"
                },
                new()
                {
                    BlockId = Guid.NewGuid(),
                    Type = ContentBlockType.Image,
                    Content = "https://example.com/image.jpg"
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllContentBlocksOfBlogQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contentBlocks);

            var result = await _controller.Index(blogId);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatIndexReturnsNoContentWhenNoContentBlocksExist()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllContentBlocksOfBlogQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.Index(blogId);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatIndexReturnsNoContentWhenContentBlocksAreNull()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllContentBlocksOfBlogQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<ContentBlockDto>)null!);

            var result = await _controller.Index(blogId);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatAddImagesReturnsOkWhenContentBlocksAreAdded()
        {
            var blogId = Guid.NewGuid();

            var request = new CreateContentBlocksRequest(
            [
                new BlockDataDto
                {
                    Type = ContentBlockType.Text,
                    Content = "First content"
                },
                new BlockDataDto
                {
                    Type = ContentBlockType.Image,
                    Content = "https://example.com/image.jpg"
                }
            ]);

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _controller.AddImages(blogId, request);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatAddImagesReturnsBadRequestWhenContentBlocksAreNotAdded()
        {
            var blogId = Guid.NewGuid();

            var request = new CreateContentBlocksRequest([]);

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _controller.AddImages(blogId, request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatUpdateImagesReturnsOkWhenContentBlocksAreUpdated()
        {
            var blogId = Guid.NewGuid();

            var request = new EditContentBlocksRequest
            (
            [
                new EditContentBlockDto
                {
                    BlockId = Guid.NewGuid(),
                    Content = "Updated content"
                },
                new EditContentBlockDto
                {
                    BlockId = Guid.NewGuid(),
                    Content = "Updated second content"
                }
            ]);

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<EditContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _controller.UpdateImages(blogId, request);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatUpdateImagesReturnsBadRequestWhenContentBlocksAreNotUpdated()
        {
            var blogId = Guid.NewGuid();

            var request = new EditContentBlocksRequest([]);

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<EditContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _controller.UpdateImages(blogId, request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatDeleteImagesReturnsOkWhenContentBlocksAreDeleted()
        {
            var blogId = Guid.NewGuid();

            var request = new DeleteContentBlocksRequest(
                new HashSet<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                });

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _controller.DeleteImages(blogId, request);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatDeleteImagesReturnsBadRequestWhenContentBlocksAreNotDeleted()
        {
            var blogId = Guid.NewGuid();

            var request = new DeleteContentBlocksRequest(
                new HashSet<Guid>());

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteContentBlocksCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _controller.DeleteImages(blogId, request);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }

}
