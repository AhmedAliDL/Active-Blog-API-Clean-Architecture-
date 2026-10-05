using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Reports.Commands.UpdateReport
{
    public class UpdateReportHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateReportHandler> logger) : IRequestHandler<UpdateReportCommand, int>
    {
        public async Task<int> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Update report operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            Report report = await unitOfWork.Reports.GetByIdAsync(request.ReportId) ?? throw new NotFoundException("Report not found.");
            if (report.Status == request.Status)
                throw new ArgumentException("Report status is already set to the requested value.");
            if (report.Status == Domain.Enums.ReportStatus.Resolved || report.Status == Domain.Enums.ReportStatus.Rejected)
                throw new ArgumentException("Report has already been resolved or rejected and cannot be updated.");
            report.Status = request.Status;
            report.ReviewerId = userId;
            report.ReviewedAt = DateTime.UtcNow;

            unitOfWork.Reports.Update(report);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Update report operation completed.");
            return result;
        }
    }
}
