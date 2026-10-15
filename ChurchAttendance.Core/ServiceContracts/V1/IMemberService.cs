using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IMemberService
{
    Task<PagedResult<MemberDto>> GetPagedAsync(MemberListQueryDto query);
    Task<MemberDto> GetByIdAsync(Guid id);
    Task<MemberDto> CreateAsync(CreateMemberDto dto);
    Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto dto);
    Task<MemberDto> PatchStatusAsync(Guid id, int status);
    Task SoftDeleteAsync(Guid id);
}
