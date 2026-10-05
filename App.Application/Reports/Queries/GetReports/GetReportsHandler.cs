using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Reports.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Reports.Queries.GetReports
{
    public class GetReportsHandler(IUnitOfWork unitOfWork, ILogger<GetReportsHandler> logger) : IRequestHandler<GetReportsQuery, List<ReportDto>>
    {
        public async Task<List<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get reports operation started.");
            List<ReportDto> result = [.. (await unitOfWork.Reports.GetAllAsync(cancellationToken))
                    .Select(r => new ReportDto
                    {
                        Id = r.Id,
                        Reason = r.Reason,
                        Description = r.Description,
                        Status = r.Status,
                        CreatedAt = r.CreatedAt,
                        ReviewedAt = r.ReviewedAt,
                        ReporterId = r.ReporterId,
                        ReviewedById = r.ReviewerId,
                        BlogId = r.BlogId
                    })];
            logger.LogInformation("Get reports operation completed.");
            return result;
        }
    }
}
