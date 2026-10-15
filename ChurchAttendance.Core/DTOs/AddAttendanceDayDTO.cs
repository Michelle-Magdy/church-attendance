namespace ChurchAttendance.Core.DTOs;

public class AddAttendanceDayDTO
{
    public DateTime Date { get; set; }
    public string? Title { get; set; } = string.Empty;
}
