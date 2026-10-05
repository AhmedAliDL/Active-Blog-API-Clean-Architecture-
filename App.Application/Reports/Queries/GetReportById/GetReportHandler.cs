using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Reports.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Reports.Queries.GetReportById
{
    public class GetReportHandler(IUnitOfWork unitOfWork, ILogger<GetReportHandler> logger) : IRequestHandler<GetReportByIdQuery, ReportDto>
    {
        public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get report operation started.");
            var report = await unitOfWork.Reports.GetByIdAsync(request.ReportId) ??
                throw new NotFoundException("Report is not found.");

            var res = new ReportDto
            {
                Id = report.Id,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                ReviewedAt = report.ReviewedAt,
                ReporterId = report.ReporterId,
                ReviewedById = report.ReviewerId,
                BlogId = report.BlogId
            };
            logger.LogInformation("Get report operation completed.");
            return res;

        }
    }
}
