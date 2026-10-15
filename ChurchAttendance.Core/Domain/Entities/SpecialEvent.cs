using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.Domain.Entities;

public class SpecialEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public Guid? LinkedAttendanceDayId { get; set; }
    public V1AttendanceDay? LinkedAttendanceDay { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
