using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories;

public class AttendanceDayRepository : IAttendanceDayRepository
{
    private readonly AppDbContext _db;

    public AttendanceDayRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AttendanceDay?> GetByIdAsync(Guid id)
        => await _db.AttendanceDays.AsNoTracking().FirstOrDefaultAsync(d => d.AttendanceDayId == id);

    public Task<bool> ExistsAsync(Guid id)
        => _db.AttendanceDays.AnyAsync(d => d.AttendanceDayId == id);

    public Task<int> CountAsync() => _db.AttendanceDays.CountAsync();

    public async Task<PagedResult<AttendanceDayListItemDTO>> GetPagedWithCountsAsync(int page, int pageSize)
    {
        var total = await _db.AttendanceDays.CountAsync();

        var items = await _db.AttendanceDays
            .AsNoTracking()
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new AttendanceDayListItemDTO
            {
                AttendanceDayId = d.AttendanceDayId,
                Title = d.Title ?? "",
                Date = d.Date,
                IsOpen = d.IsOpen,
                AttendeesCount = _db.Attendances.Count(a => a.AttendanceDayId == d.AttendanceDayId)
            })
            .ToListAsync();

        return new PagedResult<AttendanceDayListItemDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public Task<AttendanceDayListItemDTO?> GetLastWithCountAsync()
        => _db.AttendanceDays
            .AsNoTracking()
            .OrderByDescending(d => d.Date)
            .ThenByDescending(d => d.CreatedAt)
            .Select(d => new AttendanceDayListItemDTO
            {
                AttendanceDayId = d.AttendanceDayId,
                Title = d.Title ?? "",
                Date = d.Date,
                IsOpen = d.IsOpen,
                AttendeesCount = _db.Attendances.Count(a => a.AttendanceDayId == d.AttendanceDayId)
            })
            .FirstOrDefaultAsync();

    public async Task<PagedResult<MyAttendanceDayDTO>> GetPagedForUserAsync(
        Guid userId, DateTime userCreatedAtUtc, int page, int pageSize)
    {
        // Days before the user joined are not shown as "missed", unless they attended them anyway.
        var joinedOn = DateTime.SpecifyKind(userCreatedAtUtc.Date, DateTimeKind.Utc);

        var query = _db.AttendanceDays
            .AsNoTracking()
            .GroupJoin(
                _db.Attendances.Where(a => a.UserId == userId),
                d => d.AttendanceDayId,
                a => a.AttendanceDayId,
                (d, attendances) => new { d, attendances })
            .SelectMany(
                x => x.attendances.DefaultIfEmpty(),
                (x, attendance) => new { x.d, attendance })
            .Where(x => x.d.Date >= joinedOn || x.attendance != null);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.d.Date)
            .ThenByDescending(x => x.d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MyAttendanceDayDTO
            {
                AttendanceDayId = x.d.AttendanceDayId,
                Title = x.d.Title ?? "",
                Date = x.d.Date,
                IsOpen = x.d.IsOpen,
                Attended = x.attendance != null,
                MarkedAt = x.attendance != null ? x.attendance.MarkedAt : (DateTime?)null
            })
            .ToListAsync();

        return new PagedResult<MyAttendanceDayDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<AttendanceDay> AddAsync(AttendanceDay attendanceDay)
    {
        if (attendanceDay.AttendanceDayId == Guid.Empty)
            attendanceDay.AttendanceDayId = Guid.NewGuid();

        attendanceDay.CreatedAt = DateTime.UtcNow;
        attendanceDay.QrCode = QrCodeFormat.Build(attendanceDay.AttendanceDayId);
        attendanceDay.IsOpen = true;

        await _db.AttendanceDays.AddAsync(attendanceDay);
        await _db.SaveChangesAsync();
        return attendanceDay;
    }

    public async Task<AttendanceDay> UpdateAsync(AttendanceDay attendanceDay)
    {
        _db.AttendanceDays.Update(attendanceDay);
        await _db.SaveChangesAsync();
        return attendanceDay;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var day = await _db.AttendanceDays.FindAsync(id);
        if (day is null) return false;

        _db.AttendanceDays.Remove(day);
        await _db.SaveChangesAsync();
        return true;
    }
}
