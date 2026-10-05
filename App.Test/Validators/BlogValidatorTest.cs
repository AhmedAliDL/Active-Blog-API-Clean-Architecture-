using App.Application.Blogs.Commands.CreateBlog;
using App.Application.Blogs.Commands.DeleteBlog;
using App.Application.Blogs.Commands.UpdateBlog;
using App.Application.Blogs.Queries.GetBlogById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    public class BlogValidatorTest
    {
        private CreateBlogValidator _createValidator = null!;
        private DeleteBlogValidator _deleteValidator = null!;
        private UpdateBlogValidator _updateValidator = null!;
        private GetBlogByIdValidator _getBlogByIdValidator = null!;
        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateBlogValidator();
            _deleteValidator = new DeleteBlogValidator();
            _updateValidator = new UpdateBlogValidator();
            _getBlogByIdValidator = new GetBlogByIdValidator();
        }

        [Test]
        [Category("Blog")]
        public async Task EnsureCreateBlogCommandTitleWithMinimumLength()
        {
            var command = new CreateBlogCommand
            {
                Title = "A",
                CategoryId = Guid.NewGuid(),
                ImagePath = "image.jpg"
            };
            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);

        }

        [Test]
        [Category("Blog")]
        public async Task EnsureCreateBlogCommandTitleWithMaximumLength()
        {
            var command = new CreateBlogCommand
            {
                Title = "Axopjeiofjeiofjkeniu  i3r83fi h3jfh3",
                CategoryId = Guid.NewGuid(),
                ImagePath = "image.jpg"
            };
            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatCreateBlogCommandIsValid()
        {
            var command = new CreateBlogCommand
            {
                Title = "Valid Title",
                CategoryId = Guid.NewGuid(),
                ImagePath = "image.jpg"
            };
            var result = await _createValidator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatCreateBlogCommandIsInValid()
        {
            var command = new CreateBlogCommand
            {
                Title = "",
                CategoryId = Guid.Empty,
                ImagePath = ""
            };
            var result = await _createValidator.TestValidateAsync(command);
            result.ShouldHaveValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureDeleteBlogCommandIsValid()
        {
            var command = new DeleteBlogCommand(Guid.NewGuid());
            var result = await _deleteValidator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureDeleteBlogCommandIsInValid()
        {
            var command = new DeleteBlogCommand(Guid.Empty);
            var result = await _deleteValidator.TestValidateAsync(command);
            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Blog")]
        public async Task EnsureUpdateBlogCommandTitleWithMinimumLength()
        {
            var command = new UpdateBlogCommand
            {
                BlogId = Guid.NewGuid(),
                Title = "A"
            };
            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);

        }

        [Test]
        [Category("Blog")]
        public async Task EnsureUpdateBlogCommandTitleWithMaximumLength()
        {
            var command = new UpdateBlogCommand
            {
                BlogId = Guid.NewGuid(),
                Title = "Axopjeiofjeiofjkeniu  i3r83fi h3jfh3"
            };
            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogCommandIsValid()
        {
            var command = new UpdateBlogCommand
            {
                BlogId = Guid.NewGuid(),
                Title = null,
                CategoryId = null,
                ImagePath = ""
            };
            var result = await _updateValidator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogCommandIsInValid()
        {
            var command = new UpdateBlogCommand
            {
                BlogId = Guid.Empty,
                Title = "",
                CategoryId = Guid.Empty,
                ImagePath = ""
            };
            var result = await _updateValidator.TestValidateAsync(command);
            result.ShouldHaveValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetBlogByIdCommandIsValid()
        {
            var command = new GetBlogByIdQuery(Guid.NewGuid());
            var result = await _getBlogByIdValidator.TestValidateAsync(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetBlogByIdCommandIsInValid()
        {
            var command = new GetBlogByIdQuery(Guid.Empty);
            var result = await _getBlogByIdValidator.TestValidateAsync(command);
            result.ShouldHaveValidationErrors();
        }

    }

}
