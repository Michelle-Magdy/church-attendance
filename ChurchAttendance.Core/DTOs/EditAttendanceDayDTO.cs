namespace ChurchAttendance.Core.DTOs;

/// <summary>Only the fields that are supplied are changed. The id comes from the route.</summary>
public class EditAttendanceDayDTO
{
    public string? Title { get; set; }
    public DateTime? Date { get; set; }
}
