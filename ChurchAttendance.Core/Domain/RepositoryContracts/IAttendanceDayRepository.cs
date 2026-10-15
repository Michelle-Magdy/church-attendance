using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Domain.RepositoryContracts;

public interface IAttendanceDayRepository
{
    Task<AttendanceDay?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();

    /// <summary>Admin list: newest first, with attendee counts.</summary>
    Task<PagedResult<AttendanceDayListItemDTO>> GetPagedWithCountsAsync(int page, int pageSize);

    /// <summary>The most recent day (by date) with its attendee count.</summary>
    Task<AttendanceDayListItemDTO?> GetLastWithCountAsync();

    /// <summary>A user's own list of days with attended / not attended.</summary>
    Task<PagedResult<MyAttendanceDayDTO>> GetPagedForUserAsync(Guid userId, DateTime userCreatedAtUtc, int page, int pageSize);

    Task<AttendanceDay> AddAsync(AttendanceDay attendanceDay);
    Task<AttendanceDay> UpdateAsync(AttendanceDay attendanceDay);
    Task<bool> DeleteAsync(Guid id);
}
