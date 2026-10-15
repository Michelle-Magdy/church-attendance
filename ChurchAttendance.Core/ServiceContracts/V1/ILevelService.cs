using ChurchAttendance.Core.DTOs.V1;

namespace ChurchAttendance.Core.ServiceContracts.V1;

public interface ILevelService
{
    Task<List<LevelDto>> GetAllAsync();
}
