using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IV1AttendanceDayService
{
    Task<PagedResult<AttendanceDayDto>> GetPagedAsync(AttendanceDayListQueryDto query);
    Task<AttendanceDayDto> GetByIdAsync(Guid id);
    Task<AttendanceDayDto> CreateAsync(CreateAttendanceDayDto dto, string performedBy);
    Task<AttendanceDayDto> UpdateAsync(Guid id, UpdateAttendanceDayDto dto, string performedBy);
    Task<AttendanceDayDto> OpenAsync(Guid id, string performedBy);
    Task<AttendanceDayDto> CloseAsync(Guid id, string performedBy);
    Task<AttendanceDayDto> FinalizeAsync(Guid id, string performedBy);
    Task<RefreshQrResponseDto> RefreshQrAsync(Guid id);
    Task DeleteAsync(Guid id, string performedBy);

    Task<List<AttendanceListItemDto>> GetAttendeesAsync(Guid dayId, string? search);
    Task<List<AttendanceListItemDto>> GetAbsenteesAsync(Guid dayId, string? search);
    Task<ManualAttendanceResponseDto> AddManualAttendanceAsync(Guid dayId, ManualAttendanceDto dto, string performedBy);
    Task RemoveAttendanceAsync(Guid dayId, Guid userId, string? reason, string performedBy);
}
