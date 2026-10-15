using ChurchAttendance.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/attendances")]
public class AdminAttendancesController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AdminAttendancesController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    // DELETE: api/admin/attendances/{attendanceId}   (undo an attendance)
    [HttpDelete("{attendanceId:guid}")]
    public async Task<IActionResult> Delete(Guid attendanceId)
    {
        await _attendanceService.DeleteAsync(attendanceId);
        return NoContent();
    }
}
