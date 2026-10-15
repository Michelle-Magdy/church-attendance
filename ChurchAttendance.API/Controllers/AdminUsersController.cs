using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using ChurchAttendance.Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/admin/users?search=&page=1&pageSize=20   (totalCount = total users)
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserListItemDTO>>> GetUsers(
        [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _userService.GetPagedAsync(search, page, pageSize));

    // GET: api/admin/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDTO>> GetUser(Guid id)
        => Ok(await _userService.GetByIdAsync(id));

    // POST: api/admin/users
    [HttpPost]
    public async Task<ActionResult<UserResponseDTO>> CreateUser(CreateUserDTO dto)
    {
        var created = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetUser), new { id = created.Id }, created);
    }

    // PUT: api/admin/users/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponseDTO>> UpdateUser(Guid id, UpdateUserDTO dto)
        => Ok(await _userService.UpdateAsync(id, dto));

    // PATCH: api/admin/users/{id}/status   { "isActive": false }
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, SetUserStatusDTO dto)
    {
        await _userService.SetStatusAsync(id, dto.IsActive);
        return NoContent();
    }

    // POST: api/admin/users/{id}/reset-password   { "newPassword": "..." }
    [HttpPost("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, ResetUserPasswordDTO dto)
    {
        await _userService.ResetPasswordAsync(id, dto.NewPassword!);
        return NoContent();
    }

    // GET: api/admin/users/{id}/attendance-days?page=1&pageSize=20
    [HttpGet("{id:guid}/attendance-days")]
    public async Task<ActionResult<PagedResult<MyAttendanceDayDTO>>> GetUserAttendanceDays(
        Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = Paging.DefaultPageSize)
        => Ok(await _userService.GetAttendanceDaysAsync(id, page, pageSize));
}
