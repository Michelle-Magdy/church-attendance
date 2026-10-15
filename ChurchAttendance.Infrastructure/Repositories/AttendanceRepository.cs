using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChurchAttendance.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext _db;

    public AttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Attendance?> GetByIdAsync(Guid id)
        => await _db.Attendances.FindAsync(id);

    public async Task<Attendance?> TryAddAsync(Attendance attendance)
    {
        if (attendance.AttendanceId == Guid.Empty)
            attendance.AttendanceId = Guid.NewGuid();

        attendance.MarkedAt = DateTime.UtcNow;

        try
        {
            await _db.Attendances.AddAsync(attendance);
            await _db.SaveChangesAsync();
            return attendance;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // unique index (UserId, AttendanceDayId): already recorded
            _db.Entry(attendance).State = EntityState.Detached;
            return null;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var attendance = await _db.Attendances.FindAsync(id);
        if (attendance is null) return false;

        _db.Attendances.Remove(attendance);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> HasAnyForDayAsync(Guid attendanceDayId)
        => _db.Attendances.AnyAsync(a => a.AttendanceDayId == attendanceDayId);

    public Task<int> CountForDayAsync(Guid attendanceDayId)
        => _db.Attendances.CountAsync(a => a.AttendanceDayId == attendanceDayId);

    public Task<int> CountAbsentForDayAsync(Guid attendanceDayId, DateTime dayDate)
        => AbsentQuery(attendanceDayId, dayDate, null).CountAsync();

    public async Task<PagedResult<AttendeeDTO>> GetAttendeesAsync(
        Guid attendanceDayId, string? search, int page, int pageSize)
    {
        var query = _db.Attendances
            .AsNoTracking()
            .Where(a => a.AttendanceDayId == attendanceDayId)
            .Join(_db.Users, a => a.UserId, u => u.Id, (a, u) => new { a, u });

        var pattern = ToPattern(search);
        if (pattern is not null)
        {
            query = query.Where(x =>
                EF.Functions.ILike(x.u.PersonName!, pattern) ||
                EF.Functions.ILike(x.u.PhoneNumber!, pattern));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.a.MarkedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AttendeeDTO
            {
                AttendanceId = x.a.AttendanceId,
                UserId = x.u.Id,
                PersonName = x.u.PersonName ?? "",
                PhoneNumber = x.u.PhoneNumber ?? "",
                MarkedAt = x.a.MarkedAt,
                Method = x.a.Method
            })
            .ToListAsync();

        return new PagedResult<AttendeeDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResult<UserListItemDTO>> GetAbsenteesAsync(
        Guid attendanceDayId, DateTime dayDate, string? search, int page, int pageSize)
    {
        var query = AbsentQuery(attendanceDayId, dayDate, search);
        var total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.PersonName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDTO
            {
                Id = u.Id,
                PersonName = u.PersonName ?? "",
                PhoneNumber = u.PhoneNumber ?? "",
                Confessor = u.Confessor ?? "",
                IsActive = u.isActive
            })
            .ToListAsync();

        return new PagedResult<UserListItemDTO>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<AttendanceSummaryDTO> GetUserSummaryAsync(Guid userId, DateTime userCreatedAtUtc)
    {
        var joinedOn = DateTime.SpecifyKind(userCreatedAtUtc.Date, DateTimeKind.Utc);

        var attended = await _db.Attendances.CountAsync(a => a.UserId == userId);

        // A day only counts against the user once it is closed, and only if it is not before the user joined.
        var absent = await _db.AttendanceDays.CountAsync(d =>
            !d.IsOpen &&
            d.Date >= joinedOn &&
            !_db.Attendances.Any(a => a.UserId == userId && a.AttendanceDayId == d.AttendanceDayId));

        return new AttendanceSummaryDTO { TotalAttended = attended, TotalAbsent = absent };
    }

    // ------------------------------------------------------------------ helpers

    private IQueryable<ApplicationUser> AbsentQuery(Guid attendanceDayId, DateTime dayDate, string? search)
    {
        // users who joined on or before the end of that day
        var dayEnd = DateTime.SpecifyKind(dayDate.Date, DateTimeKind.Utc).AddDays(1);

        var query = _db.Users
            .AsNoTracking()
            .Where(u => u.isActive && u.CreatedAt < dayEnd)
            .Where(u => !_db.Attendances.Any(a => a.AttendanceDayId == attendanceDayId && a.UserId == u.Id));

        var pattern = ToPattern(search);
        if (pattern is not null)
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.PersonName!, pattern) ||
                EF.Functions.ILike(u.PhoneNumber!, pattern));
        }

        return query;
    }

    /// <summary>Builds an ILIKE pattern, escaping the wildcard characters typed by the user.</summary>
    internal static string? ToPattern(string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return null;

        var escaped = search.Trim()
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");

        return $"%{escaped}%";
    }
}
