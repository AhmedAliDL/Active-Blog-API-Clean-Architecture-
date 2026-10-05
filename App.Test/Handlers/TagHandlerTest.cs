using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Tags.Commands.CreateTag;
using App.Application.Tags.Commands.DeleteTag;
using App.Application.Tags.Commands.UpdateTag;
using App.Application.Tags.Queries.GetAllTagsOfCategory;
using App.Application.Tags.Queries.GetTagById;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class TagHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;

        private Mock<ILogger<CreateTagHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteTagHandler>> _deleteLogger = null!;
        private Mock<ILogger<UpdateTagHandler>> _updateLogger = null!;
        private Mock<ILogger<GetAllTagsOfCategoryHandler>> _getAllLogger = null!;
        private Mock<ILogger<GetTagByIdHandler>> _getByIdLogger = null!;

        private CreateTagHandler _createHandler = null!;
        private DeleteTagHandler _deleteHandler = null!;
        private UpdateTagHandler _updateHandler = null!;
        private GetAllTagsOfCategoryHandler _getAllHandler = null!;
        private GetTagByIdHandler _getByIdHandler = null!;

        [SetUp]
        public void SetUp()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _createLogger = new Mock<ILogger<CreateTagHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteTagHandler>>();
            _updateLogger = new Mock<ILogger<UpdateTagHandler>>();
            _getAllLogger = new Mock<ILogger<GetAllTagsOfCategoryHandler>>();
            _getByIdLogger = new Mock<ILogger<GetTagByIdHandler>>();

            _createHandler = new CreateTagHandler(
                _unitOfWork.Object,
                _createLogger.Object);

            _deleteHandler = new DeleteTagHandler(
                _unitOfWork.Object,
                _deleteLogger.Object);

            _updateHandler = new UpdateTagHandler(
                _unitOfWork.Object,
                _updateLogger.Object);

            _getAllHandler = new GetAllTagsOfCategoryHandler(
                _unitOfWork.Object,
                _getAllLogger.Object);

            _getByIdHandler = new GetTagByIdHandler(
                _unitOfWork.Object,
                _getByIdLogger.Object);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatCreateTagReturnsResultWhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var category = new Category
            {
                Id = categoryId
            };

            var command = new CreateTagCommand(
                categoryId,
                "CSharp");

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _unitOfWork
                .Setup(x => x.Tags.AddAsync(
                    It.IsAny<Tag>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Tags.AddAsync(
                    It.Is<Tag>(t =>
                        t.Name == "CSharp" &&
                        t.CategoryId == categoryId),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatCreateTagThrowsNotFoundWhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var command = new CreateTagCommand(
                categoryId,
                "CSharp");

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            // Act & Assert
            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagReturnsResultWhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var tag = new Tag
            {
                Id = tagId,
                Name = "CSharp"
            };

            var command = new DeleteTagCommand(tagId);

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync(tag);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Tags.Delete(tag),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagThrowsNotFoundWhenTagDoesNotExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var command = new DeleteTagCommand(tagId);

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync((Tag?)null);

            // Act & Assert
            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagReturnsResultWhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var oldCategoryId = Guid.NewGuid();
            var newCategoryId = Guid.NewGuid();

            var tag = new Tag
            {
                Id = tagId,
                Name = "OldName",
                CategoryId = oldCategoryId
            };

            var command = new UpdateTagCommand(
                tagId,
                newCategoryId,
                "NewName");

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync(tag);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));

            Assert.That(tag.Name, Is.EqualTo("NewName"));
            Assert.That(tag.CategoryId, Is.EqualTo(newCategoryId));
            Assert.That(tag.UpdatedAt, Is.Not.Null);

            _unitOfWork.Verify(
                x => x.Tags.Update(tag),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagKeepsOldNameWhenTagNameIsNull()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var tag = new Tag
            {
                Id = tagId,
                Name = "OldName",
                CategoryId = categoryId
            };

            var command = new UpdateTagCommand(
                tagId,
                Guid.Empty,
                null);

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync(tag);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));

            Assert.That(tag.Name, Is.EqualTo("OldName"));
            Assert.That(tag.CategoryId, Is.EqualTo(categoryId));
            Assert.That(tag.UpdatedAt, Is.Not.Null);

            _unitOfWork.Verify(
                x => x.Tags.Update(tag),
                Times.Once);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagKeepsOldCategoryWhenCategoryIdIsEmpty()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var tag = new Tag
            {
                Id = tagId,
                Name = "OldName",
                CategoryId = categoryId
            };

            var command = new UpdateTagCommand(
                tagId,
                Guid.Empty,
                "NewName");

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync(tag);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(1));

            Assert.That(tag.Name, Is.EqualTo("NewName"));
            Assert.That(tag.CategoryId, Is.EqualTo(categoryId));
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagThrowsNotFoundWhenTagDoesNotExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var command = new UpdateTagCommand(
                tagId,
                Guid.NewGuid(),
                "NewName");

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync((Tag?)null);

            // Act & Assert
            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetAllTagsReturnsMappedTags()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var tags = new List<Tag>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "CSharp",
                    CategoryId = categoryId
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "DotNet",
                    CategoryId = categoryId
                }
            };

            var query = new GetAllTagsOfCategoryQuery(categoryId);

            _unitOfWork
                .Setup(x => x.Tags.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(tags);

            // Act
            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));

            Assert.That(result[0].TagId, Is.EqualTo(tags[0].Id));
            Assert.That(result[0].TagName, Is.EqualTo("CSharp"));
            Assert.That(result[0].CategoryId, Is.EqualTo(categoryId));

            Assert.That(result[1].TagId, Is.EqualTo(tags[1].Id));
            Assert.That(result[1].TagName, Is.EqualTo("DotNet"));
            Assert.That(result[1].CategoryId, Is.EqualTo(categoryId));
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetAllTagsReturnsEmptyListWhenNoTagsExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();

            var query = new GetAllTagsOfCategoryQuery(categoryId);

            _unitOfWork
                .Setup(x => x.Tags.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Tag, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Tag>());

            // Act
            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetTagByIdReturnsTagDetailsWhenTagExists()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var tag = new Tag
            {
                Id = tagId,
                Name = "CSharp",
                CategoryId = categoryId
            };

            var query = new GetTagByIdQuery(tagId);

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync(tag);

            // Act
            var result = await _getByIdHandler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TagId, Is.EqualTo(tagId));
            Assert.That(result.TagName, Is.EqualTo("CSharp"));
            Assert.That(result.CategoryId, Is.EqualTo(categoryId));
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetTagByIdReturnsEmptyDtoWhenTagDoesNotExist()
        {
            // Arrange
            var tagId = Guid.NewGuid();

            var query = new GetTagByIdQuery(tagId);

            _unitOfWork
                .Setup(x => x.Tags.GetByIdAsync(tagId))
                .ReturnsAsync((Tag?)null);

            // Act
            var result = await _getByIdHandler.Handle(
                query,
                CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TagId, Is.EqualTo(Guid.Empty));
            Assert.That(result.TagName, Is.EqualTo(string.Empty));
            Assert.That(result.CategoryId, Is.EqualTo(Guid.Empty));
        }
    }

}
