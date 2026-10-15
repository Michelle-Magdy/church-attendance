namespace ChurchAttendance.Core.Domain.Entities;

public class Level
{
    public string Id { get; set; } = string.Empty;   // e.g. "level-servants"
    public string Name { get; set; } = string.Empty;  // e.g. "خدام"
    public int SortOrder { get; set; }
}
