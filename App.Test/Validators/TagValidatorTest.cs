using App.Application.Tags.Commands.CreateTag;
using App.Application.Tags.Commands.DeleteTag;
using App.Application.Tags.Commands.UpdateTag;
using App.Application.Tags.Queries.GetAllTagsOfCategory;
using App.Application.Tags.Queries.GetTagById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class TagValidatorTest
    {
        private CreateTagValidator _createValidator = null!;
        private DeleteTagValidator _deleteValidator = null!;
        private UpdateTagValidator _updateValidator = null!;
        private GetAllTagsOfCategoryValidator _getAllValidator = null!;
        private GetTagByIdValidator _getByIdValidator = null!;

        [SetUp]
        public void SetUp()
        {
            _createValidator = new CreateTagValidator();
            _deleteValidator = new DeleteTagValidator();
            _updateValidator = new UpdateTagValidator();
            _getAllValidator = new GetAllTagsOfCategoryValidator();
            _getByIdValidator = new GetTagByIdValidator();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatCreateTagIsValid()
        {
            // Arrange
            var command = new CreateTagCommand(
                Guid.NewGuid(),
                "CSharp");

            // Act
            var result = await _createValidator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatCreateTagIsInValidWhenTagNameIsLessThanMinimumLength()
        {
            // Arrange
            var command = new CreateTagCommand(
                Guid.NewGuid(),
                "C");

            // Act
            var result = await _createValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagName);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatCreateTagIsInValidWhenTagNameIsGreaterThanMaximumLength()
        {
            // Arrange
            var command = new CreateTagCommand(
                Guid.NewGuid(),
                "VeryLongTagName");

            // Act
            var result = await _createValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagName);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagIsValid()
        {
            // Arrange
            var command = new DeleteTagCommand(Guid.NewGuid());

            // Act
            var result = await _deleteValidator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatDeleteTagIsInValidWhenTagIdIsEmpty()
        {
            // Arrange
            var command = new DeleteTagCommand(Guid.Empty);

            // Act
            var result = await _deleteValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagId);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsValid()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "CSharp");

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsInValidWhenTagIdIsEmpty()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.Empty,
                Guid.NewGuid(),
                "CSharp");

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagId);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsInValidWhenCategoryIdIsEmpty()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.NewGuid(),
                Guid.Empty,
                "CSharp");

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsInValidWhenTagNameIsLessThanMinimumLength()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "C");

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagName);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsInValidWhenTagNameIsGreaterThanMaximumLength()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "VeryLongTagName");

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagName);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatUpdateTagIsValidWhenTagNameIsNull()
        {
            // Arrange
            var command = new UpdateTagCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null);

            // Act
            var result = await _updateValidator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetAllTagsOfCategoryIsValid()
        {
            // Arrange
            var query = new GetAllTagsOfCategoryQuery(
                Guid.NewGuid());

            // Act
            var result = await _getAllValidator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetAllTagsOfCategoryIsInValidWhenCategoryIdIsEmpty()
        {
            // Arrange
            var query = new GetAllTagsOfCategoryQuery(
                Guid.Empty);

            // Act
            var result = await _getAllValidator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CatId);
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetTagByIdIsValid()
        {
            // Arrange
            var query = new GetTagByIdQuery(
                Guid.NewGuid());

            // Act
            var result = await _getByIdValidator.TestValidateAsync(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Tag")]

        public async Task EnsureThatGetTagByIdIsInValidWhenTagIdIsEmpty()
        {
            // Arrange
            var query = new GetTagByIdQuery(
                Guid.Empty);

            // Act
            var result = await _getByIdValidator.TestValidateAsync(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TagId);
        }
    }

}
