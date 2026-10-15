using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/dashboard")]
public class V1DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public V1DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>GET /api/v1/dashboard/summary?from=2026-10-01&amp;to=2026-10-15</summary>
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        [FromQuery] string? from, [FromQuery] string? to)
        => Ok(await _service.GetSummaryAsync(from, to));

    /// <summary>GET /api/v1/dashboard/attendance-trend</summary>
    [HttpGet("attendance-trend")]
    public async Task<ActionResult<List<AttendanceTrendDto>>> GetAttendanceTrend(
        [FromQuery] string? from, [FromQuery] string? to)
        => Ok(await _service.GetAttendanceTrendAsync(from, to));

    /// <summary>GET /api/v1/dashboard/recent-attendance-days?limit=5</summary>
    [HttpGet("recent-attendance-days")]
    public async Task<ActionResult<List<AttendanceDayDto>>> GetRecentAttendanceDays(
        [FromQuery] int limit = 5)
        => Ok(await _service.GetRecentAttendanceDaysAsync(limit));

    /// <summary>GET /api/v1/dashboard/audit-logs?limit=10</summary>
    [HttpGet("audit-logs")]
    public async Task<ActionResult<List<AuditLogDto>>> GetAuditLogs(
        [FromQuery] int limit = 10)
        => Ok(await _service.GetAuditLogsAsync(limit));
}
