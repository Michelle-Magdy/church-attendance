using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class V1AttendanceDayRepository : IV1AttendanceDayRepository
{
    private readonly AppDbContext _db;

    public V1AttendanceDayRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<V1AttendanceDay>> GetPagedAsync(AttendanceDayListQueryDto query)
    {
        var q = _db.V1AttendanceDays.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim().ToLower();
            q = q.Where(d =>
                d.Title.ToLower().Contains(s) ||
                (d.Location != null && d.Location.ToLower().Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && query.Status != "all" &&
            Enum.TryParse<AttendanceDayStatus>(query.Status, true, out var statusFilter))
            q = q.Where(d => d.Status == statusFilter);

        if (!string.IsNullOrWhiteSpace(query.From) && DateOnly.TryParse(query.From, out var fromDate))
            q = q.Where(d => d.Date >= fromDate);

        if (!string.IsNullOrWhiteSpace(query.To) && DateOnly.TryParse(query.To, out var toDate))
            q = q.Where(d => d.Date <= toDate);

        var total = await q.CountAsync();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await q
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<V1AttendanceDay>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public Task<V1AttendanceDay?> GetByIdAsync(Guid id)
        => _db.V1AttendanceDays.FirstOrDefaultAsync(d => d.Id == id);

    public Task<List<V1AttendanceDay>> GetRecentAsync(int limit)
        => _db.V1AttendanceDays
            .AsNoTracking()
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.CreatedAtUtc)
            .Take(limit)
            .ToListAsync();

    public Task<List<V1AttendanceDay>> GetFinalizedInRangeAsync(DateOnly? from, DateOnly? to)
    {
        var q = _db.V1AttendanceDays
            .AsNoTracking()
            .Where(d => d.Status == AttendanceDayStatus.Finalized);

        if (from.HasValue) q = q.Where(d => d.Date >= from.Value);
        if (to.HasValue) q = q.Where(d => d.Date <= to.Value);

        return q.OrderBy(d => d.Date).ToListAsync();
    }

    public async Task<V1AttendanceDay> AddAsync(V1AttendanceDay day)
    {
        if (day.Id == Guid.Empty) day.Id = Guid.NewGuid();
        day.CreatedAtUtc = DateTime.UtcNow;
        _db.V1AttendanceDays.Add(day);
        await _db.SaveChangesAsync();
        return day;
    }

    public async Task UpdateAsync(V1AttendanceDay day)
    {
        _db.V1AttendanceDays.Update(day);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var day = await _db.V1AttendanceDays.FindAsync(id);
        if (day is null) return;
        _db.V1AttendanceDays.Remove(day);
        await _db.SaveChangesAsync();
    }

    public Task<int> CountAttendeesByDayAsync(Guid dayId)
        => _db.V1Attendances.CountAsync(a => a.AttendanceDayId == dayId);
}
