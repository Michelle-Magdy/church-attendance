using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts.V1;
using ChurchAttendance.Core.Shared;

namespace ChurchAttendance.Core.Services.V1;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _repo;

    public MemberService(IMemberRepository repo)
    {
        _repo = repo;
    }

    public Task<PagedResult<MemberDto>> GetPagedAsync(MemberListQueryDto query)
        => _repo.GetPagedAsync(query);

    public async Task<MemberDto> GetByIdAsync(Guid id)
    {
        var member = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Member not found.");
        return ToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
    {
        ValidatePhone(dto.PhoneNumber);

        if (await _repo.PhoneExistsAsync(dto.PhoneNumber))
            throw new ConflictException("This phone number is already registered.");

        var status = dto.Status.HasValue ? (UserStatus)dto.Status.Value : UserStatus.Pending;

        var member = new Member
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Gender = (Gender)dto.Gender,
            Age = dto.Age,
            LevelId = string.IsNullOrWhiteSpace(dto.LevelId) ? null : dto.LevelId,
            ConfessionFather = dto.ConfessionFather?.Trim(),
            Address = dto.Address?.Trim(),
            Status = status,
            Role = UserRole.User,
            CreatedAtUtc = DateTime.UtcNow
        };

        var added = await _repo.AddAsync(member);
        return ToDto(added);
    }

    public async Task<MemberDto> UpdateAsync(Guid id, UpdateMemberDto dto)
    {
        var member = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Member not found.");

        if (dto.Name is not null) member.Name = dto.Name.Trim();
        if (dto.Age.HasValue) member.Age = dto.Age;
        if (dto.LevelId is not null) member.LevelId = string.IsNullOrWhiteSpace(dto.LevelId) ? null : dto.LevelId;
        if (dto.ConfessionFather is not null) member.ConfessionFather = dto.ConfessionFather.Trim();
        if (dto.Address is not null) member.Address = dto.Address.Trim();

        await _repo.UpdateAsync(member);
        return ToDto(member);
    }

    public async Task<MemberDto> PatchStatusAsync(Guid id, int status)
    {
        var member = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Member not found.");

        var newStatus = (UserStatus)status;
        member.Status = newStatus;

        if (newStatus == UserStatus.Active && member.ApprovedAtUtc is null)
            member.ApprovedAtUtc = DateTime.UtcNow;

        await _repo.UpdateAsync(member);
        return ToDto(member);
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var member = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Member not found.");

        member.IsDeleted = true;
        member.Status = UserStatus.Suspended;
        await _repo.UpdateAsync(member);
    }

    private static void ValidatePhone(string phone)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(phone ?? string.Empty, @"^01[0125]\d{8}$"))
            throw new BadRequestException("Enter a valid Egyptian phone number (e.g., 01012345678).");
    }

    internal static MemberDto ToDto(Member m) => new()
    {
        Id = m.Id.ToString(),
        Name = m.Name,
        PhoneNumber = m.PhoneNumber,
        Gender = (int)m.Gender,
        Age = m.Age,
        LevelId = m.LevelId,
        LevelName = m.Level?.Name,
        ConfessionFather = m.ConfessionFather,
        Address = m.Address,
        Status = (int)m.Status,
        Role = (int)m.Role,
        CreatedAtUtc = m.CreatedAtUtc,
        ApprovedAtUtc = m.ApprovedAtUtc,
        IsDeleted = m.IsDeleted
    };
}
