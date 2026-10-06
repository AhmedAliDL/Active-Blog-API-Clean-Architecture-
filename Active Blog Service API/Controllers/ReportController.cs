using App.Application.Reports.Commands.CreateReport;
using App.Application.Reports.Commands.UpdateReport;
using App.Application.Reports.Dto;
using App.Application.Reports.HttpRequests;
using App.Application.Reports.Queries.GetReportById;
using App.Application.Reports.Queries.GetReports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Active_Blog_Service_API.Controllers
{

    /// <summary>
    /// Handles blog reports.
    /// </summary>
    public class ReportController(ISender mediator) : BaseController(mediator)
    {
        /// <summary>
        /// Gets all submitted reports.
        /// </summary>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The list of reports, or no content when no reports exist.</returns>
        [ProducesResponseType(typeof(List<ReportDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "admin")]
        [HttpGet("reports")]
        public async Task<IActionResult> GetAllReports(CancellationToken cancellationToken = default)
        {

            var reports = await _mediator.Send(new GetReportsQuery(), cancellationToken);
            if (reports != null && reports.Count > 0)
                return Ok(reports);
            return NoContent();

        }
        /// <summary>
        /// Gets a report by its unique identifier.
        /// </summary>
        /// <param name="id">The report identifier.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>The report details, or no content when the report is not found.</returns>
        [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "admin")]
        [HttpGet("reports/{id:guid}")]
        public async Task<IActionResult> GetReportById(Guid id, CancellationToken cancellationToken = default)
        {

            var report = await _mediator.Send(new GetReportByIdQuery(id), cancellationToken);
            if (report != null)
                return Ok(report);
            return NoContent();

        }
        /// <summary>
        /// Submits a report for a blog.
        /// </summary>
        /// <param name="id">The blog identifier.</param>
        /// <param name="req">The report details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when submission fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost("blogs/{id:guid}/reports")]
        public async Task<IActionResult> AddReport(Guid id, [FromBody] CreateReportRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new CreateReportCommand
            {
                Description = req.Description,
                BlogId = id,
                Reason = req.Reason,
            }, cancellationToken);
            if (result > 0)
                return Ok("Add Report Successfully");
            return BadRequest("Can`t send add report now try again later.");


        }
        /// <summary>
        /// Updates the status of a report.
        /// </summary>
        /// <param name="id">The report identifier.</param>
        /// <param name="req">The report update details.</param>
        /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
        /// <returns>A confirmation message, or an error message when the update fails.</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "admin")]
        [HttpPut("reports/{id:guid}")]
        public async Task<IActionResult> UpdateReport(Guid id, [FromBody] UpdateReportRequest req, CancellationToken cancellationToken = default)
        {

            var result = await _mediator.Send(new UpdateReportCommand(id, req.Status), cancellationToken);
            if (result > 0)
                return Ok("Report updated successfuly");
            return BadRequest("Can`t send update report now try again later.");

        }


    }
}
