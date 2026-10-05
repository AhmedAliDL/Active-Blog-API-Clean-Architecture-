using App.Application.Comments.Command.CreateComment;
using App.Application.Comments.Command.DeleteComment;
using App.Application.Comments.Command.UpdateComment;
using App.Application.Comments.Queries.GetAllCommentOfBlog;
using App.Application.Comments.Queries.GetCommentById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class CommentValidatorTest
    {
        private CreateCommentValidator _createValidator = null!;
        private DeleteCommentValidator _deleteValidator = null!;
        private UpdateCommentValidator _updateValidator = null!;
        private GetCommentOfBlogValidator _getAllValidator = null!;
        private GetCommentByIdValidator _getByIdValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateCommentValidator();
            _deleteValidator = new DeleteCommentValidator();
            _updateValidator = new UpdateCommentValidator();
            _getAllValidator = new GetCommentOfBlogValidator();
            _getByIdValidator = new GetCommentByIdValidator();
        }

        // =========================================
        // Create Comment Validator
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCommandIsValid()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = "Good article"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCommandIsInValidWhenBlogIdIsEmpty()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.Empty,
                CommentContent = "Good article"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCommandIsInValidWhenContentIsTooShort()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = ""
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentContent);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCommandIsInValidWhenContentIsTooLong()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = new string('A', 301)
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentContent);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCommandIsValidAtMaximumLength()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = new string('A', 300)
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        // =========================================
        // Delete Comment Validator
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentCommandIsValid()
        {
            var command = new DeleteCommentCommand(
                Guid.NewGuid(),
                Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentCommandIsInValidWhenCommentIdIsEmpty()
        {
            var command = new DeleteCommentCommand(
                Guid.Empty,
                Guid.NewGuid());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentId);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentCommandIsInValidWhenBlogIdIsEmpty()
        {
            var command = new DeleteCommentCommand(
                Guid.NewGuid(),
                Guid.Empty);

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        // =========================================
        // Update Comment Validator
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentCommandIsValid()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                "Updated comment",
                Guid.NewGuid());

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentCommandIsInValidWhenCommentIdIsEmpty()
        {
            var command = new UpdateCommentCommand(
                Guid.Empty,
                "Updated comment",
                Guid.NewGuid());

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentId);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentCommandIsInValidWhenBlogIdIsEmpty()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                "Updated comment",
                Guid.Empty);

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentCommandIsInValidWhenContentIsTooShort()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                "",
                Guid.NewGuid());

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentContent);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentCommandIsInValidWhenContentIsTooLong()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                new string('A', 301),
                Guid.NewGuid());

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.CommentContent);
        }

        // =========================================
        // Get Comments Of Blog Validator
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentsOfBlogQueryIsValid()
        {
            var query = new GetCommentsOfBlogQuery(Guid.NewGuid());

            var result = await _getAllValidator.TestValidateAsync(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentsOfBlogQueryIsInValid()
        {
            var query = new GetCommentsOfBlogQuery(Guid.Empty);

            var result = await _getAllValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        // =========================================
        // Get Comment By Id Validator
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentByIdQueryIsValid()
        {
            var query = new GetCommentByIdQuery(Guid.NewGuid());

            var result = await _getByIdValidator.TestValidateAsync(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentByIdQueryIsInValid()
        {
            var query = new GetCommentByIdQuery(Guid.Empty);

            var result = await _getByIdValidator.TestValidateAsync(query);

            result.ShouldHaveValidationErrorFor(x => x.CommentId);
        }
    }

}
