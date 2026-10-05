using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Reports.Commands.CreateReport;
using App.Application.Reports.Commands.UpdateReport;
using App.Application.Reports.Queries.GetReportById;
using App.Application.Reports.Queries.GetReports;
using App.Domain.Entities;
using App.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class ReportHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;

        private Mock<ILogger<CreateReportHandler>> _createLogger = null!;
        private Mock<ILogger<UpdateReportHandler>> _updateLogger = null!;
        private Mock<ILogger<GetReportHandler>> _getReportLogger = null!;
        private Mock<ILogger<GetReportsHandler>> _getReportsLogger = null!;

        private CreateReportHandler _createHandler = null!;
        private UpdateReportHandler _updateHandler = null!;
        private GetReportHandler _getReportHandler = null!;
        private GetReportsHandler _getReportsHandler = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _currentUserService = new Mock<ICurrentUserService>();

            _createLogger = new Mock<ILogger<CreateReportHandler>>();
            _updateLogger = new Mock<ILogger<UpdateReportHandler>>();
            _getReportLogger = new Mock<ILogger<GetReportHandler>>();
            _getReportsLogger = new Mock<ILogger<GetReportsHandler>>();

            _createHandler = new CreateReportHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _createLogger.Object);

            _updateHandler = new UpdateReportHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _updateLogger.Object);

            _getReportHandler = new GetReportHandler(
                _unitOfWork.Object,
                _getReportLogger.Object);

            _getReportsHandler = new GetReportsHandler(
                _unitOfWork.Object,
                _getReportsLogger.Object);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatCreateReportHandlerReturnsSuccessWhenValidRequest()
        {
            var userId = Guid.NewGuid();

            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "Spam report",
                BlogId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.AddAsync(
                    It.IsAny<Report>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatCreateReportHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "Spam report",
                BlogId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatCreateReportHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new CreateReportCommand
            {
                Reason = ReportReason.Spam,
                Description = "Spam report",
                BlogId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerReturnsSuccessWhenValidRequest()
        {
            var userId = Guid.NewGuid();
            var reportId = Guid.NewGuid();

            var command = new UpdateReportCommand(
                reportId,
                ReportStatus.Resolved);

            var report = new Report
            {
                Id = reportId,
                Reason = ReportReason.Spam,
                Description = "Spam report",
                Status = ReportStatus.Pending,
                ReporterId = Guid.NewGuid(),
                BlogId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            _unitOfWork
                .Setup(x => x.Reports.Update(report));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(report.Status, Is.EqualTo(ReportStatus.Resolved));
            Assert.That(report.ReviewerId, Is.EqualTo(userId));
            Assert.That(report.ReviewedAt, Is.Not.Null);
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new UpdateReportCommand(
                Guid.NewGuid(),
                ReportStatus.Resolved);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new UpdateReportCommand(
                Guid.NewGuid(),
                ReportStatus.Resolved);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsNotFoundExceptionWhenReportDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var reportId = Guid.NewGuid();

            var command = new UpdateReportCommand(
                reportId,
                ReportStatus.Resolved);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync((Report?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsArgumentExceptionWhenStatusIsAlreadySet()
        {
            var userId = Guid.NewGuid();
            var reportId = Guid.NewGuid();

            var command = new UpdateReportCommand(
                reportId,
                ReportStatus.Resolved);

            var report = new Report
            {
                Id = reportId,
                Status = ReportStatus.Resolved
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsArgumentExceptionWhenReportIsResolved()
        {
            var userId = Guid.NewGuid();
            var reportId = Guid.NewGuid();

            var command = new UpdateReportCommand(
                reportId,
                ReportStatus.Pending);

            var report = new Report
            {
                Id = reportId,
                Status = ReportStatus.Resolved
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatUpdateReportHandlerThrowsArgumentExceptionWhenReportIsRejected()
        {
            var userId = Guid.NewGuid();
            var reportId = Guid.NewGuid();

            var command = new UpdateReportCommand(
                reportId,
                ReportStatus.Pending);

            var report = new Report
            {
                Id = reportId,
                Status = ReportStatus.Rejected
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportHandlerReturnsReportWhenReportExists()
        {
            var reportId = Guid.NewGuid();
            var reporterId = Guid.NewGuid();
            var blogId = Guid.NewGuid();
            var reviewerId = Guid.NewGuid();

            var report = new Report
            {
                Id = reportId,
                Reason = ReportReason.Spam,
                Description = "Spam report",
                Status = ReportStatus.Resolved,
                ReviewedAt = DateTime.UtcNow,
                ReporterId = reporterId,
                ReviewerId = reviewerId,
                BlogId = blogId
            };

            var query = new GetReportByIdQuery(reportId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            var result = await _getReportHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(reportId));
            Assert.That(result.Reason, Is.EqualTo(report.Reason));
            Assert.That(result.Description, Is.EqualTo(report.Description));
            Assert.That(result.Status, Is.EqualTo(report.Status));
            Assert.That(result.ReporterId, Is.EqualTo(reporterId));
            Assert.That(result.ReviewedById, Is.EqualTo(reviewerId));
            Assert.That(result.BlogId, Is.EqualTo(blogId));
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportHandlerThrowsNotFoundExceptionWhenReportDoesNotExist()
        {
            var reportId = Guid.NewGuid();

            var query = new GetReportByIdQuery(reportId);

            _unitOfWork
                .Setup(x => x.Reports.GetByIdAsync(reportId))
                .ReturnsAsync((Report?)null);

            await Assert.ThatAsync(
                async () => await _getReportHandler.Handle(
                    query,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportsHandlerReturnsReportsWhenReportsExist()
        {
            var reports = new List<Report>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reason = ReportReason.Spam,
                    Description = "Spam report",
                    Status = ReportStatus.Pending,
                    ReporterId = Guid.NewGuid(),
                    BlogId = Guid.NewGuid()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Reason = ReportReason.Other,
                    Description = "Other report",
                    Status = ReportStatus.Rejected,
                    ReporterId = Guid.NewGuid(),
                    BlogId = Guid.NewGuid()
                }
            };

            _unitOfWork
                .Setup(x => x.Reports.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(reports);

            var query = new GetReportsQuery();

            var result = await _getReportsHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("Report")]
        public async Task EnsureThatGetReportsHandlerReturnsEmptyListWhenNoReportsExist()
        {
            _unitOfWork
                .Setup(x => x.Reports.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var query = new GetReportsQuery();

            var result = await _getReportsHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }
    }

}
