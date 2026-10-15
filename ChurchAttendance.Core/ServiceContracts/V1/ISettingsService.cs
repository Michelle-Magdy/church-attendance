using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface ISettingsService
{
    Task<ChurchSettingsDto> GetAsync();
    Task<ChurchSettingsDto> UpdateAsync(ChurchSettingsDto dto);
}
