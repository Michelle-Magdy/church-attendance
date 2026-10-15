namespace ChurchAttendance.Core.DTOs.V1;

/// <summary>Request body for POST /api/v1/auth/register.</summary>
public class V1RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public int Gender { get; set; }
    public int? Age { get; set; }
    public string? LevelId { get; set; }
    public string? ConfessionFather { get; set; }
    public string? Address { get; set; }
}

/// <summary>Request body for POST /api/v1/auth/login.</summary>
public class V1LoginDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>Response for login and register.</summary>
public class V1AuthResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime TokenExpiresAtUtc { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAtUtc { get; set; }
}
