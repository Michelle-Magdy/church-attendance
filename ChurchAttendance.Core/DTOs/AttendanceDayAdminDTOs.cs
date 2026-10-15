namespace ChurchAttendance.Core.DTOs;

/// <summary>One row in the admin list of attendance days.</summary>
public class AttendanceDayListItemDTO
{
    public Guid AttendanceDayId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsOpen { get; set; }
    public int AttendeesCount { get; set; }
}

/// <summary>Admin day details screen. QrCode is the text encoded in the QR image (admin only).</summary>
public class AttendanceDayDetailsDTO
{
    public Guid AttendanceDayId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsOpen { get; set; }
    public int TotalAttendees { get; set; }
    public int TotalAbsent { get; set; }
    public string QrCode { get; set; } = string.Empty;
}

public class AttendanceDaysSummaryDTO
{
    public int TotalDays { get; set; }
    public AttendanceDayListItemDTO? LastDay { get; set; }
}

public class DashboardDTO
{
    public int TotalUsers { get; set; }
    public int TotalDays { get; set; }
    public AttendanceDayListItemDTO? LastDay { get; set; }
}
