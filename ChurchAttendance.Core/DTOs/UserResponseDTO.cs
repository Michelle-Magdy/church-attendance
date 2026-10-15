using ChurchAttendance.Core.Domain.Entities;

namespace ChurchAttendance.Core.DTOs;

/// <summary>Full user details for the admin details screen.</summary>
public class UserResponseDTO
{
    public Guid Id { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Confessor { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ChurchCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalAttended { get; set; }
    public int TotalAbsent { get; set; }
}

public static class UserResponseDTOExtension
{
    public static UserResponseDTO ToUserResponseDTO(
        this ApplicationUser user, string role, AttendanceSummaryDTO? summary = null) => new()
    {
        Id = user.Id,
        PersonName = user.PersonName ?? string.Empty,
        PhoneNumber = user.PhoneNumber ?? string.Empty,
        Confessor = user.Confessor ?? string.Empty,
        Address = user.Address ?? string.Empty,
        ChurchCode = user.ChurchCode ?? string.Empty,
        IsActive = user.isActive,
        Role = role,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        TotalAttended = summary?.TotalAttended ?? 0,
        TotalAbsent = summary?.TotalAbsent ?? 0
    };
}
