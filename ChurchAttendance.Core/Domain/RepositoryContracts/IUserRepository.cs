using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Domain.RepositoryContracts;

public interface IUserRepository
{
    Task<PagedResult<UserListItemDTO>> GetPagedAsync(string? search, int page, int pageSize);
    Task<int> CountAsync();
}
