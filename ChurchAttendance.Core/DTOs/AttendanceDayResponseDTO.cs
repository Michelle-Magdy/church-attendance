namespace ChurchAttendance.Core.DTOs;

/// <summary>Plain day data. Never contains the QR code text.</summary>
public class AttendanceDayResponseDTO
{
    public Guid AttendanceDayId { get; set; }
    public string? Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsOpen { get; set; }
    public DateTime CreatedAt { get; set; }
}
