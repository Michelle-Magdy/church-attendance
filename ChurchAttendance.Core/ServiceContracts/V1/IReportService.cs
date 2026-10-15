using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IReportService
{
    Task<ReportSummaryDto> GetSummaryAsync(string? from, string? to, string? levelId, string? status);
    Task<PagedResult<AttendanceReportRowDto>> GetAttendanceReportAsync(string? from, string? to, string? levelId, string? status, int page, int pageSize);
    Task<List<AbsenteeFollowUpDto>> GetAbsenteeFollowUpAsync(string? from, string? to, string? levelId, double minimumAbsenceRate);
    Task<byte[]> ExportAttendanceCsvAsync(string? from, string? to, string? levelId, string? status);
}
