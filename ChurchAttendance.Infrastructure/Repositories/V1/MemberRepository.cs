using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Shared;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories.V1;

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _db;

    public MemberRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<MemberDto>> GetPagedAsync(MemberListQueryDto query)
    {
        var q = _db.Members
            .AsNoTracking()
            .Include(m => m.Level)
            .Where(m => !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.Trim().ToLower();
            q = q.Where(m =>
                m.Name.ToLower().Contains(s) ||
                m.PhoneNumber.Contains(s) ||
                (m.ConfessionFather != null && m.ConfessionFather.ToLower().Contains(s)));
        }

        if (query.Status.HasValue)
            q = q.Where(m => (int)m.Status == query.Status.Value);

        if (query.Gender.HasValue)
            q = q.Where(m => (int)m.Gender == query.Gender.Value);

        if (!string.IsNullOrWhiteSpace(query.LevelId) && query.LevelId != "all")
            q = q.Where(m => m.LevelId == query.LevelId);

        IQueryable<Member> sorted = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
        {
            ("name", "asc") => q.OrderBy(m => m.Name),
            ("name", _) => q.OrderByDescending(m => m.Name),
            ("status", "asc") => q.OrderBy(m => m.Status),
            ("status", _) => q.OrderByDescending(m => m.Status),
            (_, "asc") => q.OrderBy(m => m.CreatedAtUtc),
            _ => q.OrderByDescending(m => m.CreatedAtUtc)
        };

        var total = await sorted.CountAsync();
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await sorted
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MemberDto
            {
                Id = m.Id.ToString(),
                Name = m.Name,
                PhoneNumber = m.PhoneNumber,
                Gender = (int)m.Gender,
                Age = m.Age,
                LevelId = m.LevelId,
                LevelName = m.Level != null ? m.Level.Name : null,
                ConfessionFather = m.ConfessionFather,
                Address = m.Address,
                Status = (int)m.Status,
                Role = (int)m.Role,
                CreatedAtUtc = m.CreatedAtUtc,
                ApprovedAtUtc = m.ApprovedAtUtc,
                IsDeleted = m.IsDeleted
            })
            .ToListAsync();

        return new PagedResult<MemberDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public Task<Member?> GetByIdAsync(Guid id)
        => _db.Members
            .Include(m => m.Level)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

    public Task<bool> PhoneExistsAsync(string phone, Guid? excludeId = null)
        => _db.Members.AnyAsync(m =>
            m.PhoneNumber == phone &&
            !m.IsDeleted &&
            (excludeId == null || m.Id != excludeId.Value));

    public async Task<Member> AddAsync(Member member)
    {
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        _db.Members.Update(member);
        await _db.SaveChangesAsync();
    }

    public Task<List<Member>> GetActiveMembersCreatedOnOrBeforeAsync(DateOnly date)
    {
        var cutoff = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).AddDays(1);
        return _db.Members
            .AsNoTracking()
            .Where(m => !m.IsDeleted && m.Status == UserStatus.Active && m.CreatedAtUtc < cutoff)
            .ToListAsync();
    }
}
