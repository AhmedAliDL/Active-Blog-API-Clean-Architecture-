using App.Application.Reports.Commands.CreateReport;
using App.Application.Reports.Commands.UpdateReport;
using App.Application.Reports.Queries.GetReportById;
using App.Domain.Enums;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class ReportValidatorTest
    {
        private CreateReportValidator _createValidator = null!;
        private UpdateReportValidator _updateValidator = null!;
        private GetReportByIdValidator _getReportByIdValidator = null!;

        [SetUp]
        public void Setup()
        {
            _createValidator = new CreateReportValidator();
            _updateValidator = new UpdateReportValidator();
            _getReportByIdValidator = new GetReportByIdValidator();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatCreateReportCommandIsValid()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "Valid report description",
                BlogId = Guid.NewGuid()
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatCreateReportCommandIsInValid()
        {
            var command = new CreateReportCommand
            {
                Reason = (ReportReason)999,
                Description = "A",
                BlogId = Guid.Empty
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureCreateReportCommandBlogIdIsRequired()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "Valid description",
                BlogId = Guid.Empty
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.BlogId);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureCreateReportCommandDescriptionWithMinimumLength()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "A",
                BlogId = Guid.NewGuid()
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureCreateReportCommandDescriptionWithMaximumLength()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = new string('A', 301),
                BlogId = Guid.NewGuid()
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureCreateReportCommandDescriptionCanBeNull()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = null,
                BlogId = Guid.NewGuid()
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureUpdateReportCommandIsValid()
        {
            var command = new UpdateReportCommand(
                Guid.NewGuid(),
                ReportStatus.Resolved);

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureUpdateReportCommandIsInValid()
        {
            var command = new UpdateReportCommand(
                Guid.Empty,
                (ReportStatus)999);

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureUpdateReportCommandReportIdIsRequired()
        {
            var command = new UpdateReportCommand(
                Guid.Empty,
                ReportStatus.Resolved);

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ReportId);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureUpdateReportCommandStatusIsValidEnum()
        {
            var command = new UpdateReportCommand(
                Guid.NewGuid(),
                (ReportStatus)999);

            var result = await _updateValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportByIdCommandIsValid()
        {
            var command = new GetReportByIdQuery(Guid.NewGuid());

            var result = await _getReportByIdValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportByIdCommandIsInValid()
        {
            var command = new GetReportByIdQuery(Guid.Empty);

            var result = await _getReportByIdValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Report")]
        public async Task EnsureGetReportByIdCommandReportIdIsRequired()
        {
            var command = new GetReportByIdQuery(Guid.Empty);

            var result = await _getReportByIdValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ReportId);
        }
    }

}
