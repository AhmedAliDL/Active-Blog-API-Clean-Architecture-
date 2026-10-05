using App.Application.Follows.Commands.CreateFollow;
using App.Application.Follows.Commands.DeleteFollow;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class FollowValidatorTest
    {
        private CreateFollowValidator _createValidator = null!;
        private DeleteFollowValidator _deleteValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateFollowValidator();
            _deleteValidator = new DeleteFollowValidator();
        }

        // ============================================================
        // CreateFollowValidator
        // ============================================================

        [Test]
        [Category("Follow")]
        public async Task EnsureCreateFollowCommandIsValid()
        {
            var command = new CreateFollowCommand(
                Guid.NewGuid());

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureCreateFollowCommandIsInValidWhenBloggerIdIsEmpty()
        {
            var command = new CreateFollowCommand(
                Guid.Empty);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BloggerId);
        }

        // ============================================================
        // DeleteFollowValidator
        // ============================================================

        [Test]
        [Category("Follow")]
        public async Task EnsureDeleteFollowCommandIsValid()
        {
            var command = new DeleteFollowCommand(
                Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Follow")]
        public async Task EnsureDeleteFollowCommandIsInValidWhenBloggerIdIsEmpty()
        {
            var command = new DeleteFollowCommand(
                Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BloggerId);
        }
    }

}
