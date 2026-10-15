using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[AllowAnonymous]
[ApiController]
[Route("api/v1/auth")]
public class V1AuthController : ControllerBase
{
    private readonly IV1AuthService _authService;

    public V1AuthController(IV1AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Register a new member. Account starts as Pending; an admin must approve it.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<V1AuthResponseDto>> Register([FromBody] V1RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Login with phone number and password. Returns JWT + refresh token.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<V1AuthResponseDto>> Login([FromBody] V1LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }
}
