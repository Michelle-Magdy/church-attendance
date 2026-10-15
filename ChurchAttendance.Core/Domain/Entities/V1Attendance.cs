using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.Domain.Entities;

/// <summary>Attendance record for the v1 API (linked to Member, not ApplicationUser).</summary>
public class V1Attendance
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public Guid AttendanceDayId { get; set; }
    public V1AttendanceDay AttendanceDay { get; set; } = null!;
    public DateTime MarkedAtUtc { get; set; }
    public V1AttendanceMethod Method { get; set; }
    public string? Notes { get; set; }
}
