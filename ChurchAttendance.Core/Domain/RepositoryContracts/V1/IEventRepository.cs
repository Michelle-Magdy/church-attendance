using ChurchAttendance.Core.Domain.Entities;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface IEventRepository
{
    Task<List<SpecialEvent>> GetAllAsync();
    Task<SpecialEvent?> GetByIdAsync(Guid id);
    Task<SpecialEvent> AddAsync(SpecialEvent ev);
    Task UpdateAsync(SpecialEvent ev);
}
