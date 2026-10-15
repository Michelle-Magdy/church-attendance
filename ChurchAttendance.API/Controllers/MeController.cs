using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers;

/// <summary>Endpoints for the signed-in member (mobile app home page and QR scan).</summary>
[Authorize]
[ApiController]
[Route("api/me")]
public class MeController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public MeController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // GET: api/me/summary
    [HttpGet("summary")]
    public async Task<ActionResult<AttendanceSummaryDTO>> GetSummary()
        => Ok(await _attendanceService.GetMySummaryAsync());

    // GET: api/me/attendance-days?page=1&pageSize=20
    [HttpGet("attendance-days")]
    public async Task<ActionResult<PagedResult<MyAttendanceDayDTO>>> GetAttendanceDays(
        [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _attendanceService.GetMyDaysAsync(page, pageSize));

    // POST: api/me/attendances/scan   { "qrCode": "church-attendance:<guid>" }
    [HttpPost("attendances/scan")]
    public async Task<ActionResult<ScanAttendanceResponseDTO>> Scan(ScanAttendanceDTO dto)
    {
        var result = await _attendanceService.ScanAsync(dto.QrCode);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
