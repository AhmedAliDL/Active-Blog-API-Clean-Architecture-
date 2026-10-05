using App.Application.Categories.Commands.CreateCategory;
using App.Application.Categories.Commands.DeleteCategory;
using App.Application.Categories.Commands.UpdateCategory;
using App.Application.Categories.Queries.GetAllCategories;
using App.Application.Categories.Queries.GetCategoryById;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class CategoryHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;

        private Mock<ILogger<CreateCategoryHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteCategoryHandler>> _deleteLogger = null!;
        private Mock<ILogger<UpdateCategoryHandler>> _updateLogger = null!;
        private Mock<ILogger<GetAllCategoriesHandler>> _getAllLogger = null!;
        private Mock<ILogger<GetCategoryByIdHandler>> _getByIdLogger = null!;

        private CreateCategoryHandler _createHandler = null!;
        private DeleteCategoryHandler _deleteHandler = null!;
        private UpdateCategoryHandler _updateHandler = null!;
        private GetAllCategoriesHandler _getAllHandler = null!;
        private GetCategoryByIdHandler _getByIdHandler = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();

            _createLogger = new Mock<ILogger<CreateCategoryHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteCategoryHandler>>();
            _updateLogger = new Mock<ILogger<UpdateCategoryHandler>>();
            _getAllLogger = new Mock<ILogger<GetAllCategoriesHandler>>();
            _getByIdLogger = new Mock<ILogger<GetCategoryByIdHandler>>();

            _createHandler = new CreateCategoryHandler(
                _unitOfWork.Object,
                _createLogger.Object);

            _deleteHandler = new DeleteCategoryHandler(
                _unitOfWork.Object,
                _deleteLogger.Object);

            _updateHandler = new UpdateCategoryHandler(
                _unitOfWork.Object,
                _updateLogger.Object);

            _getAllHandler = new GetAllCategoriesHandler(
                _unitOfWork.Object,
                _getAllLogger.Object);

            _getByIdHandler = new GetCategoryByIdHandler(
                _unitOfWork.Object,
                _getByIdLogger.Object);
        }

        // =========================================
        // Create Category Handler
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureCreateCategoryReturnsSuccess()
        {
            var command = new CreateCategoryCommand("Technology");

            _unitOfWork
                .Setup(x => x.Categories.AddAsync(
                    It.IsAny<Category>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Categories.AddAsync(
                    It.Is<Category>(c =>
                        c.CategoryName == command.CategoryName),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // =========================================
        // Delete Category Handler
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureDeleteCategoryReturnsSuccess()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Technology"
            };

            var command = new DeleteCategoryCommand(category.Id);

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Categories.GetByIdAsync(category.Id),
                Times.Once);

            _unitOfWork.Verify(
                x => x.Categories.Delete(category),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureDeleteCategoryThrowsNotFoundWhenCategoryDoesNotExist()
        {
            var categoryId = Guid.NewGuid();

            var command = new DeleteCategoryCommand(categoryId);

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        // =========================================
        // Update Category Handler
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryReturnsSuccess()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Old Category"
            };

            var command = new UpdateCategoryCommand(
                category.Id,
                "New Category");

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(category.CategoryName, Is.EqualTo("New Category"));
            Assert.That(category.UpdatedAt, Is.Not.Null);

            _unitOfWork.Verify(
                x => x.Categories.GetByIdAsync(category.Id),
                Times.Once);

            _unitOfWork.Verify(
                x => x.Categories.Update(category),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryThrowsNotFoundWhenCategoryDoesNotExist()
        {
            var categoryId = Guid.NewGuid();

            var command = new UpdateCategoryCommand(
                categoryId,
                "New Category");

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryKeepsOldNameWhenNewNameIsNull()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Old Category"
            };

            var command = new UpdateCategoryCommand(
                category.Id,
                null!);

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(category.CategoryName, Is.EqualTo("Old Category"));
            Assert.That(category.UpdatedAt, Is.Not.Null);

            _unitOfWork.Verify(
                x => x.Categories.Update(category),
                Times.Once);
        }

        // =========================================
        // Get All Categories Handler
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureGetAllCategoriesReturnsCategories()
        {
            var categories = new List<Category>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryName = "Technology"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CategoryName = "Programming"
                }
            };

            _unitOfWork
                .Setup(x => x.Categories.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var result = await _getAllHandler.Handle(
                new GetAllCategoriesQuery(),
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result[0].CategoryId,
                Is.EqualTo(categories[0].Id));

            Assert.That(
                result[0].CategoryName,
                Is.EqualTo(categories[0].CategoryName));

            Assert.That(
                result[1].CategoryId,
                Is.EqualTo(categories[1].Id));

            Assert.That(
                result[1].CategoryName,
                Is.EqualTo(categories[1].CategoryName));
        }

        [Test]
        [Category("Category")]
        public async Task EnsureGetAllCategoriesReturnsEmptyListWhenNoCategoriesExist()
        {
            _unitOfWork
                .Setup(x => x.Categories.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _getAllHandler.Handle(
                new GetAllCategoriesQuery(),
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        // =========================================
        // Get Category By Id Handler
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureGetCategoryByIdReturnsCategory()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Technology"
            };

            var query = new GetCategoryByIdQuery(category.Id);

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(category.Id))
                .ReturnsAsync(category);

            var result = await _getByIdHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.CategoryId, Is.EqualTo(category.Id));
            Assert.That(result.CategoryName, Is.EqualTo(category.CategoryName));

            _unitOfWork.Verify(
                x => x.Categories.GetByIdAsync(category.Id),
                Times.Once);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureGetCategoryByIdThrowsNotFoundWhenCategoryDoesNotExist()
        {
            var categoryId = Guid.NewGuid();

            var query = new GetCategoryByIdQuery(categoryId);

            _unitOfWork
                .Setup(x => x.Categories.GetByIdAsync(categoryId))
                .ReturnsAsync((Category?)null);

            await Assert.ThatAsync(
                async () => await _getByIdHandler.Handle(
                    query,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
    }

}

