using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface IV1AttendanceRepository
{
    Task<List<AttendanceListItemDto>> GetAttendeesAsync(Guid dayId, string? search);
    Task<List<AttendanceListItemDto>> GetAbsenteesAsync(Guid dayId, string? search);
    Task<V1Attendance?> GetByMemberAndDayAsync(Guid memberId, Guid dayId);
    Task<V1Attendance> AddAsync(V1Attendance attendance);
    Task<bool> DeleteByMemberAndDayAsync(Guid memberId, Guid dayId);
    Task<int> CountAttendeesAsync(Guid dayId);
    Task<int> CountEligibleAsync(Guid dayId, DateOnly dayDate);

    /// <summary>For reports: attendee/absentee counts per finalized day in date range.</summary>
    Task<List<AttendanceReportRowDto>> GetReportRowsAsync(DateOnly? from, DateOnly? to, string? levelId, string? statusFilter);
    Task<List<AbsenteeFollowUpDto>> GetAbsenteeFollowUpAsync(DateOnly? from, DateOnly? to, string? levelId, double minAbsenceRate);
}
