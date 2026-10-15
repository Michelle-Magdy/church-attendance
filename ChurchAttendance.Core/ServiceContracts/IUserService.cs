using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts;

public interface IUserService
{
    Task<PagedResult<UserListItemDTO>> GetPagedAsync(string? search, int page, int pageSize);
    Task<int> GetCountAsync();
    Task<UserResponseDTO> GetByIdAsync(Guid id);
    Task<UserResponseDTO> CreateAsync(CreateUserDTO dto);
    Task<UserResponseDTO> UpdateAsync(Guid id, UpdateUserDTO dto);
    Task SetStatusAsync(Guid id, bool isActive);
    Task ResetPasswordAsync(Guid id, string newPassword);
    Task<PagedResult<MyAttendanceDayDTO>> GetAttendanceDaysAsync(Guid id, int page, int pageSize);
}
