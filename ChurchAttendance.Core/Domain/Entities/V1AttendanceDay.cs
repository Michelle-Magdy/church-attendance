using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.Domain.Entities;

/// <summary>
/// Attendance day entity for the v1 API contract.
/// </summary>
public class V1AttendanceDay
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public AttendanceDayStatus Status { get; set; } = AttendanceDayStatus.Scheduled;
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public string? QrToken { get; set; }
    public DateTime? QrTokenExpiresAtUtc { get; set; }
    public DateTime? FinalizedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Cached counts (populated on finalization or computed on the fly)
    public int TotalEligibleUsers { get; set; }
    public int TotalAttendees { get; set; }
    public int TotalAbsentees { get; set; }

    public ICollection<V1Attendance> Attendances { get; set; } = new List<V1Attendance>();

    public bool IsOpen => Status == AttendanceDayStatus.Open;
}
