using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class V1AttendanceRepository : IV1AttendanceRepository
{
    private readonly AppDbContext _db;

    public V1AttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<AttendanceListItemDto>> GetAttendeesAsync(Guid dayId, string? search)
    {
        var q = _db.V1Attendances
            .AsNoTracking()
            .Where(a => a.AttendanceDayId == dayId)
            .Join(_db.Members, a => a.MemberId, m => m.Id, (a, m) => new { a, m })
            .Join(_db.Levels.DefaultIfEmpty(), x => x.m.LevelId, l => l.Id, (x, l) => new { x.a, x.m, l });

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(x => x.m.Name.ToLower().Contains(s) || x.m.PhoneNumber.Contains(s));
        }

        return q.OrderBy(x => x.a.MarkedAtUtc)
            .Select(x => new AttendanceListItemDto
            {
                UserId = x.m.Id.ToString(),
                Name = x.m.Name,
                PhoneNumber = x.m.PhoneNumber,
                Gender = (int)x.m.Gender,
                LevelName = x.l != null ? x.l.Name : null,
                IsAttended = true,
                MarkedAtUtc = x.a.MarkedAtUtc,
                Method = (int)x.a.Method
            })
            .ToListAsync();
    }

    public async Task<List<AttendanceListItemDto>> GetAbsenteesAsync(Guid dayId, string? search)
    {
        // Get the day's date to determine eligible members (joined on or before that day)
        var day = await _db.V1AttendanceDays.AsNoTracking().FirstOrDefaultAsync(d => d.Id == dayId);
        if (day is null) return new List<AttendanceListItemDto>();

        var cutoff = day.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);

        var attendeeIds = await _db.V1Attendances
            .Where(a => a.AttendanceDayId == dayId)
            .Select(a => a.MemberId)
            .ToListAsync();

        var q = _db.Members
            .AsNoTracking()
            .Include(m => m.Level)
            .Where(m => !m.IsDeleted && m.Status == UserStatus.Active &&
                        m.CreatedAtUtc < cutoff &&
                        !attendeeIds.Contains(m.Id));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(m =>
                m.Name.ToLower().Contains(s) ||
                m.PhoneNumber.Contains(s) ||
                (m.Level != null && m.Level.Name.ToLower().Contains(s)));
        }

        return await q.OrderBy(m => m.Name)
            .Select(m => new AttendanceListItemDto
            {
                UserId = m.Id.ToString(),
                Name = m.Name,
                PhoneNumber = m.PhoneNumber,
                Gender = (int)m.Gender,
                LevelName = m.Level != null ? m.Level.Name : null,
                IsAttended = false,
                MarkedAtUtc = null,
                Method = null
            })
            .ToListAsync();
    }

    public Task<V1Attendance?> GetByMemberAndDayAsync(Guid memberId, Guid dayId)
        => _db.V1Attendances.FirstOrDefaultAsync(a => a.MemberId == memberId && a.AttendanceDayId == dayId);

    public async Task<V1Attendance> AddAsync(V1Attendance attendance)
    {
        if (attendance.Id == Guid.Empty) attendance.Id = Guid.NewGuid();
        attendance.MarkedAtUtc = DateTime.UtcNow;

        try
        {
            _db.V1Attendances.Add(attendance);
            await _db.SaveChangesAsync();
            return attendance;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _db.Entry(attendance).State = EntityState.Detached;
            throw new Core.Exceptions.ConflictException("Attendance is already recorded for this member and session.");
        }
    }

    public async Task<bool> DeleteByMemberAndDayAsync(Guid memberId, Guid dayId)
    {
        var a = await _db.V1Attendances.FirstOrDefaultAsync(a => a.MemberId == memberId && a.AttendanceDayId == dayId);
        if (a is null) return false;
        _db.V1Attendances.Remove(a);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<int> CountAttendeesAsync(Guid dayId)
        => _db.V1Attendances.CountAsync(a => a.AttendanceDayId == dayId);

    public Task<int> CountEligibleAsync(Guid dayId, DateOnly dayDate)
    {
        var cutoff = dayDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);
        return _db.Members.CountAsync(m =>
            !m.IsDeleted &&
            m.Status == UserStatus.Active &&
            m.CreatedAtUtc < cutoff);
    }

    public async Task<List<AttendanceReportRowDto>> GetReportRowsAsync(
        DateOnly? from, DateOnly? to, string? levelId, string? statusFilter)
    {
        var q = _db.V1AttendanceDays.AsNoTracking();

        if (from.HasValue) q = q.Where(d => d.Date >= from.Value);
        if (to.HasValue) q = q.Where(d => d.Date <= to.Value);

        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "all" &&
            Enum.TryParse<AttendanceDayStatus>(statusFilter, true, out var sf))
            q = q.Where(d => d.Status == sf);

        var days = await q.OrderBy(d => d.Date).ToListAsync();

        // For each day, compute per-level if levelId specified, otherwise aggregate
        var result = new List<AttendanceReportRowDto>();

        foreach (var day in days)
        {
            if (!string.IsNullOrWhiteSpace(levelId) && levelId != "all")
            {
                // Count only members in that level
                var cutoff = day.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);
                var eligible = await _db.Members.CountAsync(m =>
                    !m.IsDeleted && m.Status == UserStatus.Active &&
                    m.CreatedAtUtc < cutoff && m.LevelId == levelId);

                var attendees = await _db.V1Attendances.CountAsync(a =>
                    a.AttendanceDayId == day.Id &&
                    _db.Members.Any(m => m.Id == a.MemberId && m.LevelId == levelId));

                var level = await _db.Levels.FindAsync(levelId);
                var pct = eligible > 0 ? Math.Round((double)attendees / eligible * 100, 1) : 0;

                result.Add(new AttendanceReportRowDto
                {
                    AttendanceDayId = day.Id.ToString(),
                    Title = day.Title,
                    Date = day.Date.ToString("yyyy-MM-dd"),
                    LevelId = levelId,
                    LevelName = level?.Name,
                    EligibleCount = eligible,
                    Attendees = attendees,
                    Absentees = Math.Max(0, eligible - attendees),
                    AttendancePercentage = pct
                });
            }
            else
            {
                var pct = day.TotalEligibleUsers > 0
                    ? Math.Round((double)day.TotalAttendees / day.TotalEligibleUsers * 100, 1)
                    : 0;

                result.Add(new AttendanceReportRowDto
                {
                    AttendanceDayId = day.Id.ToString(),
                    Title = day.Title,
                    Date = day.Date.ToString("yyyy-MM-dd"),
                    LevelId = null,
                    LevelName = null,
                    EligibleCount = day.TotalEligibleUsers,
                    Attendees = day.TotalAttendees,
                    Absentees = day.TotalAbsentees,
                    AttendancePercentage = pct
                });
            }
        }

        return result;
    }

    public async Task<List<AbsenteeFollowUpDto>> GetAbsenteeFollowUpAsync(
        DateOnly? from, DateOnly? to, string? levelId, double minAbsenceRate)
    {
        var dayQuery = _db.V1AttendanceDays
            .AsNoTracking()
            .Where(d => d.Status == AttendanceDayStatus.Finalized);

        if (from.HasValue) dayQuery = dayQuery.Where(d => d.Date >= from.Value);
        if (to.HasValue) dayQuery = dayQuery.Where(d => d.Date <= to.Value);

        var dayIds = await dayQuery.Select(d => d.Id).ToListAsync();

        var memberQuery = _db.Members
            .AsNoTracking()
            .Include(m => m.Level)
            .Where(m => !m.IsDeleted && m.Status == UserStatus.Active);

        if (!string.IsNullOrWhiteSpace(levelId) && levelId != "all")
            memberQuery = memberQuery.Where(m => m.LevelId == levelId);

        var members = await memberQuery.ToListAsync();

        var result = new List<AbsenteeFollowUpDto>();
        foreach (var member in members)
        {
            // Eligible sessions: finalized days on or after member joined
            var joinDate = DateOnly.FromDateTime(member.CreatedAtUtc.Date);
            var eligibleDayIds = await dayQuery
                .Where(d => d.Date >= joinDate)
                .Select(d => d.Id)
                .ToListAsync();

            var eligible = eligibleDayIds.Count;
            if (eligible == 0) continue;

            var attended = await _db.V1Attendances.CountAsync(a =>
                a.MemberId == member.Id && eligibleDayIds.Contains(a.AttendanceDayId));

            var absent = eligible - attended;
            var absenceRate = (double)absent / eligible;

            if (absenceRate >= minAbsenceRate)
            {
                result.Add(new AbsenteeFollowUpDto
                {
                    UserId = member.Id.ToString(),
                    Name = member.Name,
                    PhoneNumber = member.PhoneNumber,
                    LevelName = member.Level?.Name,
                    EligibleSessions = eligible,
                    AttendedSessions = attended,
                    AbsentSessions = absent,
                    AbsenceRate = Math.Round(absenceRate, 2)
                });
            }
        }

        return result.OrderByDescending(r => r.AbsenceRate).ToList();
    }
}
