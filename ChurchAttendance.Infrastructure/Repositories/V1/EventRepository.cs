using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _db;

    public EventRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<SpecialEvent>> GetAllAsync()
        => _db.SpecialEvents.AsNoTracking().OrderBy(e => e.StartAt).ToListAsync();

    public Task<SpecialEvent?> GetByIdAsync(Guid id)
        => _db.SpecialEvents.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<SpecialEvent> AddAsync(SpecialEvent ev)
    {
        if (ev.Id == Guid.Empty) ev.Id = Guid.NewGuid();
        ev.CreatedAtUtc = DateTime.UtcNow;
        _db.SpecialEvents.Add(ev);
        await _db.SaveChangesAsync();
        return ev;
    }

    public async Task UpdateAsync(SpecialEvent ev)
    {
        _db.SpecialEvents.Update(ev);
        await _db.SaveChangesAsync();
    }
}
