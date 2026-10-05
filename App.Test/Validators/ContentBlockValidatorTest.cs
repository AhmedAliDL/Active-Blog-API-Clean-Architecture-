using App.Application.ContentBlocks.Commands.CreateContentBlocks;
using App.Application.ContentBlocks.Commands.DeleteContentBlocks;
using App.Application.ContentBlocks.Commands.EditContentBlocks;
using App.Application.ContentBlocks.Dto;
using App.Application.ContentBlocks.Queries.GetAllContentBlocks;
using App.Domain.Enums;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class ContentBlockValidatorTest
    {
        private CreateContentBlocksValidator _createValidator = null!;
        private DeleteContentBlocksValidator _deleteValidator = null!;
        private EditContentBlocksValidator _editValidator = null!;
        private GetAllBlogImagesValidator _getAllValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateContentBlocksValidator();
            _deleteValidator = new DeleteContentBlocksValidator();
            _editValidator = new EditContentBlocksValidator();
            _getAllValidator = new GetAllBlogImagesValidator();
        }

        // ============================================================
        // CreateContentBlocksValidator
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandRequiresAtLeastTwoItems()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Data.Count);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandIsValidWhenItContainsTwoItems()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Text"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Another text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandRequiresBlogId()
        {
            var command = new CreateContentBlocksCommand(
                Guid.Empty,
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Text"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Another text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandTextContentIsRequired()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = ""
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Valid text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandImageRequiresValidUrl()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Image,
                        Content = "not-a-url"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Valid text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandVideoRequiresValidUrl()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Video,
                        Content = "not-a-url"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Valid text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandAcceptsValidImageUrl()
        {
            var command = new CreateContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Image,
                        Content = "https://example.com/image.jpg"
                    },
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Text,
                        Content = "Valid text"
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureCreateContentBlocksCommandIsInValidWhenBlogIdAndDataAreInvalid()
        {
            var command = new CreateContentBlocksCommand(
                Guid.Empty,
                [
                    new BlockDataDto
                    {
                        Type = ContentBlockType.Image,
                        Content = ""
                    }
                ]);

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        // ============================================================
        // DeleteContentBlocksValidator
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureDeleteContentBlocksCommandIsValid()
        {
            var command = new DeleteContentBlocksCommand(
                Guid.NewGuid(),
                new HashSet<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                });

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureDeleteContentBlocksCommandRequiresBlogId()
        {
            var command = new DeleteContentBlocksCommand(
                Guid.Empty,
                new HashSet<Guid>
                {
                    Guid.NewGuid(),
                    Guid.NewGuid()
                });

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BLogId);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureDeleteContentBlocksCommandRequiresAtLeastTwoIds()
        {
            var command = new DeleteContentBlocksCommand(
                Guid.NewGuid(),
                new HashSet<Guid>
                {
                    Guid.NewGuid()
                });

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ContentBlocksIds.Count);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureDeleteContentBlocksCommandIsInValidWhenBlogIdAndIdsAreInvalid()
        {
            var command = new DeleteContentBlocksCommand(
                Guid.Empty,
                new HashSet<Guid>());

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        // ============================================================
        // EditContentBlocksValidator
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandIsValid()
        {
            var command = new EditContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "First content"
                    },
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "Second content"
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandRequiresAtLeastTwoItems()
        {
            var command = new EditContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "Content"
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Data.Count);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandRequiresBlogId()
        {
            var command = new EditContentBlocksCommand(
                Guid.Empty,
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "First"
                    },
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "Second"
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandRequiresBlockId()
        {
            var command = new EditContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.Empty,
                        Content = "First"
                    },
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "Second"
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandRequiresContent()
        {
            var command = new EditContentBlocksCommand(
                Guid.NewGuid(),
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = ""
                    },
                    new EditContentBlockDto
                    {
                        BlockId = Guid.NewGuid(),
                        Content = "Second"
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureEditContentBlocksCommandIsInValidWhenBlogIdDataAndBlockIdAreInvalid()
        {
            var command = new EditContentBlocksCommand(
                Guid.Empty,
                [
                    new EditContentBlockDto
                    {
                        BlockId = Guid.Empty,
                        Content = ""
                    }
                ]);

            var result = await _editValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        // ============================================================
        // GetAllBlogImagesValidator
        // ============================================================

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureGetAllContentBlocksQueryIsValid()
        {
            var command = new GetAllContentBlocksOfBlogQuery(
                Guid.NewGuid());

            var result = await _getAllValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("ContentBlock")]
        public async Task EnsureGetAllContentBlocksQueryIsInValidWhenBlogIdIsEmpty()
        {
            var command = new GetAllContentBlocksOfBlogQuery(
                Guid.Empty);

            var result = await _getAllValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }
    }

}
