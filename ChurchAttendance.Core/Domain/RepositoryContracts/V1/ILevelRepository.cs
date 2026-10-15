using ChurchAttendance.Core.Domain.Entities;

namespace ChurchAttendance.Core.Domain.RepositoryContracts.V1;

public interface ILevelRepository
{
    Task<List<Level>> GetAllAsync();
}
