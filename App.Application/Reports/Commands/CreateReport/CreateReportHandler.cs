using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using App.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Reports.Commands.CreateReport
{
    public class CreateReportHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<CreateReportHandler> logger) : IRequestHandler<CreateReportCommand, int>
    {
        public async Task<int> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create report operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty)
                throw new NotFoundException("User not found.");
            var report = new Report
            {
                Reason = request.Reason,
                Description = request.Description ?? string.Empty,
                Status = ReportStatus.Pending,
                ReporterId = userId!.Value,
                BlogId = request.BlogId,
            };
            await unitOfWork.Reports.AddAsync(report, cancellationToken);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Create report operation completed.");
            return result;
        }
    }
}
