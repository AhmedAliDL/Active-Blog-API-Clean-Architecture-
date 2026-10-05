using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.ContentBlocks.Commands.CreateContentBlocks;
using App.Application.ContentBlocks.Commands.DeleteContentBlocks;
using App.Application.ContentBlocks.Commands.EditContentBlocks;
using App.Application.ContentBlocks.Dto;
using App.Application.ContentBlocks.Queries.GetAllContentBlocks;
using App.Domain.Entities;
using App.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class ContentBlockHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;

        private Mock<ILogger<CreateContentBlocksHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteContentBlocksHandler>> _deleteLogger = null!;
        private Mock<ILogger<GetAllContentBlocksOfBlogHandler>> _getAllLogger = null!;

        private CreateContentBlocksHandler _createHandler = null!;
        private DeleteContentBlocksHandler _deleteHandler = null!;
        private EditContentBlocksHandler _editHandler = null!;
        private GetAllContentBlocksOfBlogHandler _getAllHandler = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _currentUserService = new Mock<ICurrentUserService>();

            _createLogger =
                new Mock<ILogger<CreateContentBlocksHandler>>();

            _deleteLogger =
                new Mock<ILogger<DeleteContentBlocksHandler>>();

            _getAllLogger =
                new Mock<ILogger<GetAllContentBlocksOfBlogHandler>>();

            _createHandler = new CreateContentBlocksHandler(
                _unitOfWork.Object,
                _createLogger.Object);

            _deleteHandler = new DeleteContentBlocksHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _deleteLogger.Object);

            _editHandler = new EditContentBlocksHandler(
                _unitOfWork.Object);

            _getAllHandler = new GetAllContentBlocksOfBlogHandler(
                _unitOfWork.Object,
                _getAllLogger.Object);
        }

        // ============================================================
        // CreateContentBlocksHandler
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatCreateContentBlocksHandlerReturnsSuccessWhenValidRequest()
        {
            var blogId = Guid.NewGuid();

            var command = new CreateContentBlocksCommand(
                blogId,
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "First block"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Image,
                        Content = "https://example.com/image.jpg"
                    }
                ]);

            _unitOfWork
                .Setup(x => x.ContentBlocks.AddRangeAsync(
                    It.IsAny<List<ContentBlock>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatCreateContentBlocksHandlerAssignsCorrectOrder()
        {
            var blogId = Guid.NewGuid();

            var command = new CreateContentBlocksCommand(
                blogId,
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "First"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Heading,
                        Content = "Second"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Image,
                        Content = "https://example.com/image.jpg"
                    }
                ]);

            List<ContentBlock>? addedBlocks = null;

            _unitOfWork
                .Setup(x => x.ContentBlocks.AddRangeAsync(
                    It.IsAny<List<ContentBlock>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<ContentBlock>, CancellationToken>(
                    (blocks, _) => addedBlocks = blocks.ToList())
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(addedBlocks, Is.Not.Null);
            Assert.That(addedBlocks!.Count, Is.EqualTo(3));

            Assert.That(addedBlocks[0].Order, Is.EqualTo(1));
            Assert.That(addedBlocks[1].Order, Is.EqualTo(2));
            Assert.That(addedBlocks[2].Order, Is.EqualTo(3));

            Assert.That(addedBlocks[0].BlogId, Is.EqualTo(blogId));
            Assert.That(addedBlocks[0].Data, Is.EqualTo("First"));
            Assert.That(addedBlocks[0].Type, Is.EqualTo(ContentBlockType.Text));
        }

        // ============================================================
        // DeleteContentBlocksHandler
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatDeleteContentBlocksHandlerReturnsSuccessWhenValidRequest()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var blockId1 = Guid.NewGuid();
            var blockId2 = Guid.NewGuid();

            var command = new DeleteContentBlocksCommand(
                blogId,
                new HashSet<Guid>
                {
                    blockId1,
                    blockId2
                });

            var contentBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = blockId1,
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Block 1"
                },
                new()
                {
                    Id = blockId2,
                    BlogId = blogId,
                    Type = ContentBlockType.Image,
                    Data = "https://example.com/image.jpg"
                }
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contentBlocks);

            _unitOfWork
                .Setup(x => x.ContentBlocks.DeleteRange(contentBlocks));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatDeleteContentBlocksHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new DeleteContentBlocksCommand(
                Guid.NewGuid(),
                new HashSet<Guid>
                {
                    Guid.NewGuid()
                });

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatDeleteContentBlocksHandlerThrowsForbiddenExceptionWhenContentBlocksDoNotExist()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new DeleteContentBlocksCommand(
                blogId,
                new HashSet<Guid>
                {
                    Guid.NewGuid()
                });

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentBlock>());

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ForbiddenException>());
        }

        // ============================================================
        // EditContentBlocksHandler
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatEditContentBlocksHandlerReturnsSuccessWhenValidRequest()
        {
            var blogId = Guid.NewGuid();

            var blockId1 = Guid.NewGuid();
            var blockId2 = Guid.NewGuid();

            var existingBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = blockId1,
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Old text",
                    Order = 1
                },
                new()
                {
                    Id = blockId2,
                    BlogId = blogId,
                    Type = ContentBlockType.Image,
                    Data = "old-image.jpg",
                    Order = 2
                }
            };

            var command = new EditContentBlocksCommand(
                blogId,
                [
                    new EditContentBlockDto
                    {
                        BlockId = blockId1,
                        Content = "Updated text"
                    },
                    new EditContentBlockDto
                    {
                        BlockId = blockId2,
                        Content = "https://example.com/new-image.jpg"
                    }
                ]);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBlocks);

            _unitOfWork
                .Setup(x => x.ContentBlocks.Update(
                    It.IsAny<ContentBlock>()));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            var result = await _editHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatEditContentBlocksHandlerUpdatesOrderAndContent()
        {
            var blogId = Guid.NewGuid();

            var blockId1 = Guid.NewGuid();
            var blockId2 = Guid.NewGuid();

            var existingBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = blockId1,
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Old text",
                    Order = 1
                },
                new()
                {
                    Id = blockId2,
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Old second text",
                    Order = 2
                }
            };

            var command = new EditContentBlocksCommand(
                blogId,
                [
                    new EditContentBlockDto
                    {
                        BlockId = blockId2,
                        Content = "Updated second"
                    },
                    new EditContentBlockDto
                    {
                        BlockId = blockId1,
                        Content = "Updated first"
                    }
                ]);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBlocks);

            _unitOfWork
                .Setup(x => x.ContentBlocks.Update(
                    It.IsAny<ContentBlock>()));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            await _editHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(existingBlocks[0].Data, Is.EqualTo("Updated first"));
            Assert.That(existingBlocks[0].Order, Is.EqualTo(2));

            Assert.That(existingBlocks[1].Data, Is.EqualTo("Updated second"));
            Assert.That(existingBlocks[1].Order, Is.EqualTo(1));
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatEditContentBlocksHandlerThrowsArgumentExceptionWhenTextBlockContainsUrl()
        {
            var blogId = Guid.NewGuid();
            var blockId = Guid.NewGuid();

            var existingBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = blockId,
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Old text",
                    Order = 1
                }
            };

            var command = new EditContentBlocksCommand(
                blogId,
                [
                    new EditContentBlockDto
                    {
                        BlockId = blockId,
                        Content = "https://example.com/text"
                    }
                ]);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBlocks);

            await Assert.ThatAsync(
                async () => await _editHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatEditContentBlocksHandlerDoesNotUpdateWhenBlockDoesNotExist()
        {
            var blogId = Guid.NewGuid();

            var existingBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "Existing",
                    Order = 1
                }
            };

            var command = new EditContentBlocksCommand(
                blogId,
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "New content"
                    }
                ]);

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBlocks);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _editHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(0));

            _unitOfWork.Verify(
                x => x.ContentBlocks.Update(It.IsAny<ContentBlock>()),
                Times.Never);
        }

        // ============================================================
        // GetAllContentBlocksOfBlogHandler
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatGetAllContentBlocksHandlerReturnsContentBlocksWhenTheyExist()
        {
            var blogId = Guid.NewGuid();

            var contentBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    Type = ContentBlockType.Text,
                    Data = "First block"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    Type = ContentBlockType.Image,
                    Data = "https://example.com/image.jpg"
                }
            };

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contentBlocks);

            var query = new GetAllContentBlocksOfBlogQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatGetAllContentBlocksHandlerReturnsEmptyListWhenNoContentBlocksExist()
        {
            var blogId = Guid.NewGuid();

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentBlock>());

            var query = new GetAllContentBlocksOfBlogQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureThatGetAllContentBlocksHandlerMapsContentBlockCorrectly()
        {
            var blogId = Guid.NewGuid();
            var blockId = Guid.NewGuid();

            var contentBlocks = new List<ContentBlock>
            {
                new()
                {
                    Id = blockId,
                    BlogId = blogId,
                    Type = ContentBlockType.Heading,
                    Data = "My Heading"
                }
            };

            _unitOfWork
                .Setup(x => x.ContentBlocks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<ContentBlock, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(contentBlocks);

            var query = new GetAllContentBlocksOfBlogQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].BlockId, Is.EqualTo(blockId));
            Assert.That(result[0].Type, Is.EqualTo(ContentBlockType.Heading));
            Assert.That(result[0].Content, Is.EqualTo("My Heading"));
        }
    }

}
