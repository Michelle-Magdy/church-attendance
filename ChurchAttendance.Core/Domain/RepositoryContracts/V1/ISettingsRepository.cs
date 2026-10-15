using ChurchAttendance.Core.Domain.Entities;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface ISettingsRepository
{
    Task<ChurchSettings> GetAsync();
    Task UpdateAsync(ChurchSettings settings);
}
