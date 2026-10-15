using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface IMemberRepository
{
    Task<PagedResult<MemberDto>> GetPagedAsync(MemberListQueryDto query);
    Task<Member?> GetByIdAsync(Guid id);
    Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null);
    Task<Member> AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task<List<Member>> GetActiveMembersCreatedOnOrBeforeAsync(DateOnly date);
}
