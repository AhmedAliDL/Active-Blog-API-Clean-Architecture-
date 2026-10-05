using App.Application.Reports.Dto;
using MediatR;

namespace App.Application.Reports.Queries.GetReports
{
    public record GetReportsQuery : IRequest<List<ReportDto>>;
}
