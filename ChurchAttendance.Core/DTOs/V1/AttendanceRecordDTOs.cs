namespace ChurchAttendance.Core.DTOs.V1;

public class AttendanceListItemDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Gender { get; set; }
    public string? LevelName { get; set; }
    public bool IsAttended { get; set; }
    public DateTime? MarkedAtUtc { get; set; }
    public int? Method { get; set; }
}

public class ManualAttendanceDto
{
    public string UserId { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

public class ManualAttendanceResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AttendanceDayId { get; set; } = string.Empty;
    public DateTime MarkedAtUtc { get; set; }
    public int Method { get; set; }
    public string? Notes { get; set; }
}

public class RemoveAttendanceDto
{
    public string? Reason { get; set; }
}
