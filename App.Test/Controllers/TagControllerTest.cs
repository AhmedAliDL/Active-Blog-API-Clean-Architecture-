using Active_Blog_Service_API.Controllers;
using App.Application.Tags.Commands.CreateTag;
using App.Application.Tags.Commands.DeleteTag;
using App.Application.Tags.Commands.UpdateTag;
using App.Application.Tags.Dto;
using App.Application.Tags.HttpRequests;
using App.Application.Tags.Queries.GetAllTagsOfCategory;
using App.Application.Tags.Queries.GetTagById;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class TagControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private TagController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _controller = new TagController(_mediator.Object);
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatIndexReturnsOkWhenTagsExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var tags = new List<TagDetailsDto>
            {
                new()
                {
                    TagId = Guid.NewGuid(),
                    TagName = "CSharp",
                    CategoryId = categoryId
                }
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTagsOfCategoryQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tags);

            // Act
            var result = await _controller.Index(
                categoryId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatIndexReturnsNoContentWhenTagsAreEmpty()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTagsOfCategoryQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TagDetailsDto>());

            // Act
            var result = await _controller.Index(
                categoryId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatIndexReturnsNoContentWhenTagsAreNull()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllTagsOfCategoryQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<TagDetailsDto>)null!);

            // Act
            var result = await _controller.Index(
                categoryId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatDetailsReturnsOkWhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var tag = new TagDetailsDto
            {
                TagId = tagId,
                TagName = "CSharp",
                CategoryId = Guid.NewGuid()
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetTagByIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tag);

            // Act
            var result = await _controller.Details(
                tagId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatDetailsReturnsNoContentWhenTagIsNull()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetTagByIdQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TagDetailsDto?)null);

            // Act
            var result = await _controller.Details(
                tagId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatAddTagReturnsOkWhenTagIsAddedSuccessfully()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var request = new CreateTagRequest("CSharp");

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.AddTag(
                categoryId,
                request,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatAddTagReturnsBadRequestWhenTagIsNotAdded()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var request = new CreateTagRequest("CSharp");

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.AddTag(
                categoryId,
                request,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [Category("Tag")]
        public async Task EnsureThatEditTagReturnsOkWhenTagIsUpdatedSuccessfully()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var request = new UpdateTagRequest(
                Guid.NewGuid(),
                "CSharp");

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<UpdateTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.EditTag(
                tagId,
                request,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatEditTagReturnsBadRequestWhenTagIsNotUpdated()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var request = new UpdateTagRequest(
                Guid.NewGuid(),
                "CSharp");

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<UpdateTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.EditTag(
                tagId,
                request,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagReturnsOkWhenTagIsDeletedSuccessfully()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteTag(
                tagId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagReturnsBadRequestWhenTagIsNotDeleted()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteTagCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.DeleteTag(
                tagId,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
    }

}

