using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface IEventService
{
    Task<List<SpecialEventDto>> GetAllAsync();
    Task<SpecialEventDto> GetByIdAsync(Guid id);
    Task<SpecialEventDto> CreateAsync(CreateEventDto dto, string performedBy);
    Task<SpecialEventDto> UpdateAsync(Guid id, UpdateEventDto dto, string performedBy);
    Task<SpecialEventDto> PatchStatusAsync(Guid id, string status, string performedBy);
}
