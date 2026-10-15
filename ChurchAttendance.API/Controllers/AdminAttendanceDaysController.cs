using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/attendance-days")]
public class AdminAttendanceDaysController : ControllerBase
{
    private readonly IAttendanceDayService _dayService;
    private readonly IAttendanceService _attendanceService;

    public AdminAttendanceDaysController(IAttendanceDayService dayService, IAttendanceService attendanceService)
    {
        _dayService = dayService;
        _attendanceService = attendanceService;
    }

    // GET: api/admin/attendance-days/summary
    [HttpGet("summary")]
    public async Task<ActionResult<AttendanceDaysSummaryDTO>> GetSummary()
        => Ok(await _dayService.GetSummaryAsync());

    // GET: api/admin/attendance-days?page=1&pageSize=20   (totalCount = total days)
    [HttpGet]
    public async Task<ActionResult<PagedResult<AttendanceDayListItemDTO>>> GetDays(
        [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _dayService.GetPagedAsync(page, pageSize));

    // GET: api/admin/attendance-days/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceDayDetailsDTO>> GetDay(Guid id)
        => Ok(await _dayService.GetDetailsAsync(id));

    // POST: api/admin/attendance-days
    [HttpPost]
    public async Task<ActionResult<AttendanceDayResponseDTO>> CreateDay(AddAttendanceDayDTO dto)
    {
        var created = await _dayService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetDay), new { id = created.AttendanceDayId }, created);
    }

    // PUT: api/admin/attendance-days/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AttendanceDayResponseDTO>> UpdateDay(Guid id, EditAttendanceDayDTO dto)
        => Ok(await _dayService.UpdateAsync(id, dto));

    // PATCH: api/admin/attendance-days/{id}/status   { "isOpen": true }
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<AttendanceDayResponseDTO>> SetStatus(Guid id, SetAttendanceDayStatusDTO dto)
        => Ok(await _dayService.SetStatusAsync(id, dto.IsOpen));

    // DELETE: api/admin/attendance-days/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDay(Guid id)
    {
        await _dayService.DeleteAsync(id);
        return NoContent();
    }

    // GET: api/admin/attendance-days/{id}/qr   -> PNG image
    [HttpGet("{id:guid}/qr")]
    public async Task<IActionResult> GetQrPng(Guid id)
        => File(await _dayService.GetQrPngAsync(id), "image/png");

    // GET: api/admin/attendance-days/{id}/qr.pdf   -> printable PDF
    [HttpGet("{id:guid}/qr.pdf")]
    public async Task<IActionResult> GetQrPdf(Guid id)
        => File(await _dayService.GetQrPdfAsync(id), "application/pdf", $"attendance-qr-{id}.pdf");

    // GET: api/admin/attendance-days/{id}/attendees?search=&page=1&pageSize=20
    [HttpGet("{id:guid}/attendees")]
    public async Task<ActionResult<PagedResult<AttendeeDTO>>> GetAttendees(
        Guid id, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _dayService.GetAttendeesAsync(id, search, page, pageSize));

    // GET: api/admin/attendance-days/{id}/absentees?search=&page=1&pageSize=20
    [HttpGet("{id:guid}/absentees")]
    public async Task<ActionResult<PagedResult<UserListItemDTO>>> GetAbsentees(
        Guid id, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _dayService.GetAbsenteesAsync(id, search, page, pageSize));

    // POST: api/admin/attendance-days/{id}/attendances   { "userId": "..." }   (manual check-in)
    [HttpPost("{id:guid}/attendances")]
    public async Task<ActionResult<AttendanceResponseDTO>> AddManualAttendance(Guid id, AddManualAttendanceDTO dto)
    {
        var created = await _attendanceService.AddManualAsync(id, dto.UserId);
        return StatusCode(StatusCodes.Status201Created, created);
    }
}
