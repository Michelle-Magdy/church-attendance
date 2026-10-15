using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ReportSummaryDto>> GetSummary(
        [FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] string? levelId, [FromQuery] string? status)
        => Ok(await _service.GetSummaryAsync(from, to, levelId, status));

    [HttpGet("attendance")]
    public async Task<IActionResult> GetAttendance(
        [FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] string? levelId, [FromQuery] string? status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(await _service.GetAttendanceReportAsync(from, to, levelId, status, page, pageSize));

    [HttpGet("absentee-follow-up")]
    public async Task<ActionResult<List<AbsenteeFollowUpDto>>> GetAbsenteeFollowUp(
        [FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] string? levelId,
        [FromQuery] double minimumAbsenceRate = 0.3)
        => Ok(await _service.GetAbsenteeFollowUpAsync(from, to, levelId, minimumAbsenceRate));

    [HttpGet("attendance/export")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] string? from, [FromQuery] string? to,
        [FromQuery] string? levelId, [FromQuery] string? status)
    {
        var bytes = await _service.ExportAttendanceCsvAsync(from, to, levelId, status);
        return File(bytes, "text/csv; charset=utf-8", "attendance-report.csv");
    }
}
