using Active_Blog_Service_API.Controllers;
using App.Application.Reports.Commands.CreateReport;
using App.Application.Reports.Commands.UpdateReport;
using App.Application.Reports.Dto;
using App.Application.Reports.HttpRequests;
using App.Application.Reports.Queries.GetReportById;
using App.Application.Reports.Queries.GetReports;
using App.Domain.Entities;
using App.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
        public class ReportControllerTest
        {
            private Mock<IMediator> _mediator = null!;
            private ReportController _controller = null!;

            [SetUp]
            public void Setup()
            {
                _mediator = new Mock<IMediator>();
                _controller = new ReportController(_mediator.Object);
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatGetAllReportsReturnsOkResultWhenReportsExist()
            {
                var reports = new List<ReportDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Reason = ReportReason.Spam,
                    Description = "Spam report",
                    Status = ReportStatus.Pending,
                    ReporterId = Guid.NewGuid(),
                    BlogId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Reason = ReportReason.Other,
                    Description = "Other report",
                    Status = ReportStatus.Pending,
                    ReporterId = Guid.NewGuid(),
                    BlogId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetReportsQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(reports);

                var result = await _controller.GetAllReports();

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatGetAllReportsReturnsNoContentWhenNoReportsExist()
            {
                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetReportsQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);

                var result = await _controller.GetAllReports();

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatGetAllReportsReturnsNoContentWhenReportsAreNull()
            {
                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetReportsQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((List<ReportDto>?)null!);

                var result = await _controller.GetAllReports();

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatGetReportByIdReturnsOkResultWhenReportExists()
            {
                var reportId = Guid.NewGuid();

                var report = new ReportDto
                {
                    Id = reportId,
                    Reason = ReportReason.Spam,
                    Description = "Spam report",
                    Status = ReportStatus.Pending,
                    ReporterId = Guid.NewGuid(),
                    BlogId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetReportByIdQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(report);

                var result = await _controller.GetReportById(reportId);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatGetReportByIdReturnsNoContentWhenReportDoesNotExist()
            {
                var reportId = Guid.NewGuid();

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetReportByIdQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((ReportDto?)null!);

                var result = await _controller.GetReportById(reportId);

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatAddReportReturnsOkWhenReportIsAdded()
            {
                var blogId = Guid.NewGuid();

                var request = new CreateReportRequest
                {
                    Reason = ReportReason.Spam,
                    Description = "This is a spam report"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<CreateReportCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.AddReport(blogId, request);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatAddReportReturnsBadRequestWhenReportIsNotAdded()
            {
                var blogId = Guid.NewGuid();

                var request = new CreateReportRequest
                {
                    Reason = ReportReason.Spam,
                    Description = "This is a spam report"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<CreateReportCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.AddReport(blogId, request);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatUpdateReportReturnsOkWhenReportIsUpdated()
            {
                var reportId = Guid.NewGuid();

             var request = new UpdateReportRequest(ReportStatus.Resolved);

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<UpdateReportCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.UpdateReport(reportId, request);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Report")]
            public async Task EnsureThatUpdateReportReturnsBadRequestWhenReportIsNotUpdated()
            {
                var reportId = Guid.NewGuid();

            var request = new UpdateReportRequest(ReportStatus.Resolved);

            _mediator
                    .Setup(x => x.Send(
                        It.IsAny<UpdateReportCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.UpdateReport(reportId, request);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }
        }
    
}

