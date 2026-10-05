using App.Application.Categories.Commands.CreateCategory;
using App.Application.Categories.Commands.DeleteCategory;
using App.Application.Categories.Commands.UpdateCategory;
using App.Application.Categories.Queries.GetCategoryById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class CategoryValidatorTest
    {
        private CreateCategoryValidator _createValidator = null!;
        private DeleteCategoryValidator _deleteValidator = null!;
        private UpdateCategoryValidator _updateValidator = null!;
        private GetCategoryByIdValidator _getByIdValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateCategoryValidator();
            _deleteValidator = new DeleteCategoryValidator();
            _updateValidator = new UpdateCategoryValidator();
            _getByIdValidator = new GetCategoryByIdValidator();
        }

        // =========================================
        // Create Category Validator
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureCreateCategoryCommandIsValid()
        {
            var command = new CreateCategoryCommand("Technology");

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureCreateCategoryCommandIsInValidWhenNameIsTooShort()
        {
            var command = new CreateCategoryCommand("A");

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureCreateCategoryCommandIsInValidWhenNameIsTooLong()
        {
            var command = new CreateCategoryCommand(
                new string('A', 21));

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureCreateCategoryCommandCategoryNameHasValidLength()
        {
            var command = new CreateCategoryCommand(
                new string('A', 20));

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        // =========================================
        // Delete Category Validator
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureDeleteCategoryCommandIsValid()
        {
            var command = new DeleteCategoryCommand(Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureDeleteCategoryCommandIsInValid()
        {
            var command = new DeleteCategoryCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureDeleteCategoryCommandCategoryIdIsRequired()
        {
            var command = new DeleteCategoryCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        // =========================================
        // Update Category Validator
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryCommandIsValid()
        {
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                "Technology");

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryCommandIsInValidWhenCategoryIdIsEmpty()
        {
            var command = new UpdateCategoryCommand(
                Guid.Empty,
                "Technology");

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryCommandIsInValidWhenNameIsTooShort()
        {
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                "A");

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryName);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryCommandIsInValidWhenNameIsTooLong()
        {
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                new string('A', 21));

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CategoryName);
        }

        [Test]
        [Category("Category")]
        public async Task EnsureUpdateCategoryCommandCategoryNameHasValidLength()
        {
            var command = new UpdateCategoryCommand(
                Guid.NewGuid(),
                new string('A', 20));

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        // =========================================
        // Get Category By Id Validator
        // =========================================

        [Test]
        [Category("Category")]
        public async Task EnsureGetCategoryByIdQueryIsValid()
        {
            var query = new GetCategoryByIdQuery(Guid.NewGuid());

            var result = await _getByIdValidator.TestValidateAsync(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureGetCategoryByIdQueryIsInValid()
        {
            var query = new GetCategoryByIdQuery(Guid.Empty);

            var result = await _getByIdValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Category")]
        public async Task EnsureGetCategoryByIdQueryIdIsRequired()
        {
            var query = new GetCategoryByIdQuery(Guid.Empty);

            var result = await _getByIdValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }

}
