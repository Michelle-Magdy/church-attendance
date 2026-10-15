using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Domain.RepositoryContracts;

public interface IAttendanceRepository
{
    Task<Attendance?> GetByIdAsync(Guid id);

    /// <summary>Adds the attendance. Returns null when the user already has one for that day.</summary>
    Task<Attendance?> TryAddAsync(Attendance attendance);

    Task<bool> DeleteAsync(Guid id);
    Task<bool> HasAnyForDayAsync(Guid attendanceDayId);
    Task<int> CountForDayAsync(Guid attendanceDayId);

    /// <summary>Active users who joined on or before the day and have no attendance for it.</summary>
    Task<int> CountAbsentForDayAsync(Guid attendanceDayId, DateTime dayDate);

    Task<PagedResult<AttendeeDTO>> GetAttendeesAsync(Guid attendanceDayId, string? search, int page, int pageSize);
    Task<PagedResult<UserListItemDTO>> GetAbsenteesAsync(Guid attendanceDayId, DateTime dayDate, string? search, int page, int pageSize);

    /// <summary>
    /// Attended = all attendances of the user.
    /// Absent = closed days on or after the user's join date that the user did not attend.
    /// </summary>
    Task<AttendanceSummaryDTO> GetUserSummaryAsync(Guid userId, DateTime userCreatedAtUtc);
}
