using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;

namespace ChurchAttendance.Core.Services.V1;

public class LevelService : ILevelService
{
    private readonly ILevelRepository _repo;

    public LevelService(ILevelRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<LevelDto>> GetAllAsync()
    {
        var levels = await _repo.GetAllAsync();
        return levels.Select(l => new LevelDto
        {
            Id = l.Id,
            Name = l.Name,
            SortOrder = l.SortOrder
        }).ToList();
    }
}
