using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/attendance-days")]
public class AttendanceDaysController : ControllerBase
{
    private readonly IV1AttendanceDayService _service;

    public AttendanceDaysController(IV1AttendanceDayService service)
    {
        _service = service;
    }

    private string PerformedBy => User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "unknown";

    // ---- CRUD ----

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] AttendanceDayListQueryDto query)
        => Ok(await _service.GetPagedAsync(query));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AttendanceDayDto>> Get(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<AttendanceDayDto>> Create([FromBody] CreateAttendanceDayDto dto)
    {
        var created = await _service.CreateAsync(dto, PerformedBy);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AttendanceDayDto>> Update(Guid id, [FromBody] UpdateAttendanceDayDto dto)
        => Ok(await _service.UpdateAsync(id, dto, PerformedBy));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, PerformedBy);
        return NoContent();
    }

    // ---- Status transitions ----

    [HttpPost("{id:guid}/open")]
    public async Task<ActionResult<AttendanceDayDto>> Open(Guid id)
        => Ok(await _service.OpenAsync(id, PerformedBy));

    [HttpPost("{id:guid}/close")]
    public async Task<ActionResult<AttendanceDayDto>> Close(Guid id)
        => Ok(await _service.CloseAsync(id, PerformedBy));

    [HttpPost("{id:guid}/finalize")]
    public async Task<ActionResult<AttendanceDayDto>> Finalize(Guid id)
        => Ok(await _service.FinalizeAsync(id, PerformedBy));

    [HttpPost("{id:guid}/refresh-qr")]
    public async Task<ActionResult<RefreshQrResponseDto>> RefreshQr(Guid id)
        => Ok(await _service.RefreshQrAsync(id));

    // ---- Attendees / absentees ----

    [HttpGet("{id:guid}/attendees")]
    public async Task<ActionResult<List<AttendanceListItemDto>>> GetAttendees(
        Guid id, [FromQuery] string? search)
        => Ok(await _service.GetAttendeesAsync(id, search));

    [HttpGet("{id:guid}/absentees")]
    public async Task<ActionResult<List<AttendanceListItemDto>>> GetAbsentees(
        Guid id, [FromQuery] string? search)
        => Ok(await _service.GetAbsenteesAsync(id, search));

    [HttpPost("{id:guid}/attendees/manual")]
    public async Task<ActionResult<ManualAttendanceResponseDto>> AddManualAttendance(
        Guid id, [FromBody] ManualAttendanceDto dto)
    {
        var result = await _service.AddManualAttendanceAsync(id, dto, PerformedBy);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("{id:guid}/attendees/{userId:guid}/remove")]
    public async Task<IActionResult> RemoveAttendance(
        Guid id, Guid userId, [FromBody] RemoveAttendanceDto? dto)
    {
        await _service.RemoveAttendanceAsync(id, userId, dto?.Reason, PerformedBy);
        return NoContent();
    }
}
