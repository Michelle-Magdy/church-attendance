using ChurchAttendance.Core.Domain.RepositoryContracts;
using ChurchAttendance.Core.DTOs;
using ChurchAttendance.Core.Shared;
using Microsoft.EntityFrameworkCore;

namespace ChurchAttendance.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<int> CountAsync() => _db.Users.CountAsync();

    public async Task<PagedResult<UserListItemDTO>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = _db.Users.AsNoTracking();

        var pattern = AttendanceRepository.ToPattern(search);
        if (pattern is not null)
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.PersonName!, pattern) ||
                EF.Functions.ILike(u.PhoneNumber!, pattern));
        }

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
}
