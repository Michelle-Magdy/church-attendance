namespace ChurchAttendance.Core.DTOs;

public class UserListItemDTO
{
    public Guid Id { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Confessor { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateUserDTO
{
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public string? PersonName { get; set; }
    public string? Confessor { get; set; }
    public string? Address { get; set; }
    public string? ChurchCode { get; set; }

    /// <summary>"User" (default) or "Admin".</summary>
    public string? Role { get; set; }
}

public class UpdateUserDTO
{
    public string? PhoneNumber { get; set; }
    public string? PersonName { get; set; }
    public string? Confessor { get; set; }
    public string? Address { get; set; }
    public string? ChurchCode { get; set; }
}

public class SetUserStatusDTO
{
    public bool IsActive { get; set; }
}

public class ResetUserPasswordDTO
{
    public string? NewPassword { get; set; }
}
