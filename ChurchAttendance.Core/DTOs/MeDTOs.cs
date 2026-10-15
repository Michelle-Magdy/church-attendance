namespace ChurchAttendance.Core.DTOs;

public class AttendanceSummaryDTO
{
    public int TotalAttended { get; set; }
    public int TotalAbsent { get; set; }
}

/// <summary>One day in a user's own list. Deliberately has no QR code.</summary>
public class MyAttendanceDayDTO
{
    public Guid AttendanceDayId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsOpen { get; set; }
    public bool Attended { get; set; }
    public DateTime? MarkedAt { get; set; }
}

public class ScanAttendanceDTO
{
    public string? QrCode { get; set; }
}

public class ScanAttendanceResponseDTO
{
    public Guid AttendanceId { get; set; }
    public Guid AttendanceDayId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime MarkedAt { get; set; }
}
