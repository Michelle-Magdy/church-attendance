namespace ChurchAttendance.Core.DTOs.V1;

public class SpecialEventDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? LinkedAttendanceDayId { get; set; }
}

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; } = "Draft";
    public bool CreateLinkedAttendanceDay { get; set; }
}

public class UpdateEventDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
}

public class PatchEventStatusDto
{
    public string Status { get; set; } = string.Empty;
}
