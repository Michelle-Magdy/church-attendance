using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class SettingsRepository : ISettingsRepository
{
    private readonly AppDbContext _db;

    public SettingsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ChurchSettings> GetAsync()
    {
        var s = await _db.ChurchSettings.FirstOrDefaultAsync();
        if (s is null)
        {
            s = new ChurchSettings { Id = 1 };
            _db.ChurchSettings.Add(s);
            await _db.SaveChangesAsync();
        }
        return s;
    }

    public async Task UpdateAsync(ChurchSettings settings)
    {
        _db.ChurchSettings.Update(settings);
        await _db.SaveChangesAsync();
    }
}
