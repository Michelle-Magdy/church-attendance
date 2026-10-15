using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface IV1AttendanceDayRepository
{
    Task<PagedResult<V1AttendanceDay>> GetPagedAsync(AttendanceDayListQueryDto query);
    Task<V1AttendanceDay?> GetByIdAsync(Guid id);
    Task<List<V1AttendanceDay>> GetRecentAsync(int limit);
    Task<List<V1AttendanceDay>> GetFinalizedInRangeAsync(DateOnly? from, DateOnly? to);
    Task<V1AttendanceDay> AddAsync(V1AttendanceDay day);
    Task UpdateAsync(V1AttendanceDay day);
    Task DeleteAsync(Guid id);
    Task<int> CountAttendeesByDayAsync(Guid dayId);
}
