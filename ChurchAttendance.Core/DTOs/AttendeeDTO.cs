using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.DTOs;

public class AttendeeDTO
{
    public Guid AttendanceId { get; set; }
    public Guid UserId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime MarkedAt { get; set; }
    public AttendanceMethod Method { get; set; }
}
