using System.Security.Claims;
using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ChurchAttendance.API.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;
    private readonly ICurrentUserService _currentUser;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jwtService,
        ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _currentUser = currentUser;
    }

    // POST: api/Auth/login
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponseDTO>> PostLogin(LoginDTO loginDto)
    {
        const string wrongCredentials = "Wrong phone number or password.";

        var user = await _userManager.FindByNameAsync(loginDto.PhoneNumber!);
        if (user is null || !user.isActive)
            return Unauthorized(Problem401(wrongCredentials));

        var check = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, lockoutOnFailure: true);
        if (check.IsLockedOut)
            return Unauthorized(Problem401("Too many failed attempts. Try again later."));
        if (!check.Succeeded)
            return Unauthorized(Problem401(wrongCredentials));

        return Ok(await IssueTokensAsync(user));
    }

    // POST: api/Auth/register
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponseDTO>> PostRegister(RegisterDTO registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.PhoneNumber,
            PhoneNumber = registerDto.PhoneNumber,
            PersonName = registerDto.PersonName?.Trim(),
            Confessor = registerDto.Confessor?.Trim(),
            Address = registerDto.Address?.Trim(),
            ChurchCode = registerDto.ChurchCode?.Trim(),
            isActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password!);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            return BadRequest(new ValidationProblemDetails(errors));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return StatusCode(StatusCodes.Status201Created, await IssueTokensAsync(user));
    }

    // POST: api/Auth/refresh
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticationResponseDTO>> PostRefresh(RefreshRequestDTO request)
    {
        ClaimsPrincipal? principal;
        try
        {
            principal = _jwtService.GetClaimsPrincipal(request.Token);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return Unauthorized(Problem401("Invalid access token."));
        }

        var userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal?.FindFirstValue("sub");
        if (userId is null)
            return Unauthorized(Problem401("Invalid access token."));

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null
            || !user.isActive
            || string.IsNullOrEmpty(user.RefreshToken)
            || user.RefreshToken != request.RefreshToken
            || user.RefreshTokenExpirationDate < DateTime.UtcNow)
        {
            return Unauthorized(Problem401("Invalid refresh token."));
        }

        return Ok(await IssueTokensAsync(user));
    }

    // POST: api/Auth/logout  (revokes the refresh token)
    [HttpPost("logout")]
    public async Task<IActionResult> PostLogout()
    {
        var id = _currentUser.userId;
        if (id is not null)
        {
            var user = await _userManager.FindByIdAsync(id.Value.ToString());
            if (user is not null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpirationDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        return NoContent();
    }

    private async Task<AuthenticationResponseDTO> IssueTokensAsync(ApplicationUser user)
    {
        var response = await _jwtService.CreateJWTTokenAsync(user);
        user.RefreshToken = response.RefreshToken;
        user.RefreshTokenExpirationDate = response.RefreshTokenExpirationDate;
        await _userManager.UpdateAsync(user);
        return response;
    }

    private static ProblemDetails Problem401(string detail) => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Unauthorized",
        Detail = detail
    };
}
