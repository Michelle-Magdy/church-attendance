using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts;

public interface IAttendanceService
{
    // ----- current user -----
    Task<AttendanceSummaryDTO> GetMySummaryAsync();
    Task<PagedResult<MyAttendanceDayDTO>> GetMyDaysAsync(int page, int pageSize);
    Task<ScanAttendanceResponseDTO> ScanAsync(string? qrCode);

    // ----- admin -----
    Task<AttendanceResponseDTO> AddManualAsync(Guid attendanceDayId, Guid userId);
    Task DeleteAsync(Guid attendanceId);
}
