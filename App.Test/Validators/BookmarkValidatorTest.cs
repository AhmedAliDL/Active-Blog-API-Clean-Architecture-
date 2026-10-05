using App.Application.Bookmarks.Commands.CreateBookmark;
using App.Application.Bookmarks.Commands.DeleteBookmark;
using App.Domain.Entities;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class BookmarkValidatorTest
    {
        private CreateBookmarkValidator _createValidator = null!;
        private DeleteBookmarkValidator _deleteValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateBookmarkValidator();
            _deleteValidator = new DeleteBookmarkValidator();
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureCreateBookmarkCommandIsValid()
        {
            var command = new CreateBookmarkCommand(Guid.NewGuid());

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureCreateBookmarkCommandIsInValid()
        {
            var command = new CreateBookmarkCommand(Guid.Empty);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureCreateBookmarkCommandBlogIdIsRequired()
        {
            var command = new CreateBookmarkCommand(Guid.Empty);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureDeleteBookmarkCommandIsValid()
        {
            var command = new DeleteBookmarkCommand(Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureDeleteBookmarkCommandIsInValid()
        {
            var command = new DeleteBookmarkCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureDeleteBookmarkCommandBlogIdIsRequired()
        {
            var command = new DeleteBookmarkCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }
    }

}
