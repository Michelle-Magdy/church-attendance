namespace ChurchAttendance.Core.DTOs.V1;

public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }
    public int TotalPendingUsers { get; set; }
    public int TotalAttendanceDays { get; set; }
    public int TotalAttendees { get; set; }
    public int TotalAbsentees { get; set; }
    public string? LastAttendanceDayDate { get; set; }
    public int LastDayAttendees { get; set; }
    public int LastDayAbsentees { get; set; }
    public double AttendancePercentage { get; set; }
}

public class AttendanceTrendDto
{
    public string Date { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Attendees { get; set; }
    public int Absentees { get; set; }
    public double AttendancePercentage { get; set; }
}

public class AuditLogDto
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? TargetId { get; set; }
    public string? Details { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
