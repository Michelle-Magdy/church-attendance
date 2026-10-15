namespace ChurchAttendance.Core.DTOs.V1;

public class AttendanceDayDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;          // YYYY-MM-DD
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string Status { get; set; } = string.Empty;        // Scheduled | Open | Closed | Finalized
    public bool IsOpen { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public int TotalEligibleUsers { get; set; }
    public int TotalAttendees { get; set; }
    public int TotalAbsentees { get; set; }
    public double AttendancePercentage { get; set; }
    public string? QrToken { get; set; }
    public DateTime? QrTokenExpiresAtUtc { get; set; }
    public DateTime? FinalizedAtUtc { get; set; }
}

public class CreateAttendanceDayDto
{
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public bool IsOpen { get; set; }
}

public class UpdateAttendanceDayDto
{
    public string? Title { get; set; }
    public string? Date { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class RefreshQrResponseDto
{
    public string QrToken { get; set; } = string.Empty;
    public DateTime QrTokenExpiresAtUtc { get; set; }
}

public class AttendanceDayListQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
}
