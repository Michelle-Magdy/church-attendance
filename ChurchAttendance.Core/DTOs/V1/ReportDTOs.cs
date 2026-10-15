using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.DTOs.V1;

public class ReportSummaryDto
{
    public string? From { get; set; }
    public string? To { get; set; }
    public int TotalSessions { get; set; }
    public int FinalizedSessions { get; set; }
    public int TotalAttendees { get; set; }
    public int TotalAbsentees { get; set; }
    public double AttendancePercentage { get; set; }
    public int MembersNeedingFollowUp { get; set; }
}

public class AttendanceReportRowDto
{
    public string AttendanceDayId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? LevelId { get; set; }
    public string? LevelName { get; set; }
    public int EligibleCount { get; set; }
    public int Attendees { get; set; }
    public int Absentees { get; set; }
    public double AttendancePercentage { get; set; }
}

public class AbsenteeFollowUpDto
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? LevelName { get; set; }
    public int EligibleSessions { get; set; }
    public int AttendedSessions { get; set; }
    public int AbsentSessions { get; set; }
    public double AbsenceRate { get; set; }
}
