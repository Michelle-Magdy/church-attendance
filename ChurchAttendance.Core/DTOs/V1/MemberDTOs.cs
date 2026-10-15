using ChurchAttendance.Core.Domain.Enums;

namespace ChurchAttendance.Core.DTOs.V1;

public class MemberDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Gender { get; set; }
    public int? Age { get; set; }
    public string? LevelId { get; set; }
    public string? LevelName { get; set; }
    public string? ConfessionFather { get; set; }
    public string? Address { get; set; }
    public int Status { get; set; }
    public int Role { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}

public class CreateMemberDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Gender { get; set; }
    public int? Age { get; set; }
    public string? LevelId { get; set; }
    public string? ConfessionFather { get; set; }
    public string? Address { get; set; }
    public int? Status { get; set; }
}

public class UpdateMemberDto
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? LevelId { get; set; }
    public string? ConfessionFather { get; set; }
    public string? Address { get; set; }
}

public class PatchMemberStatusDto
{
    public int Status { get; set; }
}

public class MemberListQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public int? Status { get; set; }
    public int? Gender { get; set; }
    public string? LevelId { get; set; }
    public string SortBy { get; set; } = "createdAtUtc";
    public string SortDirection { get; set; } = "desc";
}
