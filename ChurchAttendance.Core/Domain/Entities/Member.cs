using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.Domain.Entities;

/// <summary>
/// Front-end member entity that maps to the frontend's User DTO.
/// Separate from ApplicationUser so Identity is not polluted with domain fields.
/// </summary>
public class Member
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int? Age { get; set; }
    public string? LevelId { get; set; }
    public Level? Level { get; set; }
    public string? ConfessionFather { get; set; }
    public string? Address { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pending;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<V1Attendance> Attendances { get; set; } = new List<V1Attendance>();
}
