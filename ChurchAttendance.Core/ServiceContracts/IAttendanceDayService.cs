using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts;

public interface IAttendanceDayService
{
    Task<PagedResult<AttendanceDayListItemDTO>> GetPagedAsync(int page, int pageSize);
    Task<AttendanceDaysSummaryDTO> GetSummaryAsync();
    Task<AttendanceDayDetailsDTO> GetDetailsAsync(Guid id);
    Task<AttendanceDayResponseDTO> CreateAsync(AddAttendanceDayDTO dto);
    Task<AttendanceDayResponseDTO> UpdateAsync(Guid id, EditAttendanceDayDTO dto);
    Task<AttendanceDayResponseDTO> SetStatusAsync(Guid id, bool isOpen);
    Task DeleteAsync(Guid id);

    Task<byte[]> GetQrPngAsync(Guid id);
    Task<byte[]> GetQrPdfAsync(Guid id);

    Task<PagedResult<AttendeeDTO>> GetAttendeesAsync(Guid id, string? search, int page, int pageSize);
    Task<PagedResult<UserListItemDTO>> GetAbsenteesAsync(Guid id, string? search, int page, int pageSize);
}
