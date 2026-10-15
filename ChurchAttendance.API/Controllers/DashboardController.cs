using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAttendanceDayService _dayService;

    public DashboardController(IUserService userService, IAttendanceDayService dayService)
    {
        _userService = userService;
        _dayService = dayService;
    }

    // GET: api/admin/dashboard
    [HttpGet]
    public async Task<ActionResult<DashboardDTO>> Get()
    {
        var totalUsers = await _userService.GetCountAsync();
        var summary = await _dayService.GetSummaryAsync();

        return Ok(new DashboardDTO
        {
            TotalUsers = totalUsers,
            TotalDays = summary.TotalDays,
            LastDay = summary.LastDay
        });
    }
}
