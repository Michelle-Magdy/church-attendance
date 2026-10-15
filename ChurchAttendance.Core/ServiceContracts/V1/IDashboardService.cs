using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(string? from, string? to);
    Task<List<AttendanceTrendDto>> GetAttendanceTrendAsync(string? from, string? to);
    Task<List<AttendanceDayDto>> GetRecentAttendanceDaysAsync(int limit);
    Task<List<AuditLogDto>> GetAuditLogsAsync(int limit);
}
