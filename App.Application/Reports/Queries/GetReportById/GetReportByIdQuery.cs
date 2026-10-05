using App.Application.Reports.Dto;
using MediatR;

namespace App.Application.Reports.Queries.GetReportById
{
    public record GetReportByIdQuery(Guid ReportId) : IRequest<ReportDto>;
}
