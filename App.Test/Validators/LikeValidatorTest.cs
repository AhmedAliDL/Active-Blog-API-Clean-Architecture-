using App.Application.Likes.Commands.CreateLike;
using App.Application.Likes.Commands.DeleteLike;
using App.Application.Likes.Queries.GetAllBlogLikes;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class LikeValidatorTest
    {
        private CreateLikeValidator _createValidator = null!;
        private DeleteLikeValidator _deleteValidator = null!;
        private GetAllBlogLikeValidator _getAllValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateLikeValidator();
            _deleteValidator = new DeleteLikeValidator();
            _getAllValidator = new GetAllBlogLikeValidator();
        }

        // =========================================
        // Create Like Validator
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureCreateLikeCommandIsValid()
        {
            var command = new CreateLikeCommand(Guid.NewGuid());

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureCreateLikeCommandIsInValid()
        {
            var command = new CreateLikeCommand(Guid.Empty);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureCreateLikeCommandBlogIdIsRequired()
        {
            var command = new CreateLikeCommand(Guid.Empty);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        // =========================================
        // Delete Like Validator
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureDeleteLikeCommandIsValid()
        {
            var command = new DeleteLikeCommand(Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureDeleteLikeCommandIsInValid()
        {
            var command = new DeleteLikeCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureDeleteLikeCommandBlogIdIsRequired()
        {
            var command = new DeleteLikeCommand(Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        // =========================================
        // Get All Blog Likes Validator
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureGetAllBlogLikesQueryIsValid()
        {
            var query = new GetAllBlogLikesQuery(Guid.NewGuid());

            var result = await _getAllValidator.TestValidateAsync(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureGetAllBlogLikesQueryIsInValid()
        {
            var query = new GetAllBlogLikesQuery(Guid.Empty);

            var result = await _getAllValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Like")]
        public async Task EnsureGetAllBlogLikesQueryBlogIdIsRequired()
        {
            var query = new GetAllBlogLikesQuery(Guid.Empty);

            var result = await _getAllValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }
    }

}
