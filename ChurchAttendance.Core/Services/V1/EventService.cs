using ChurchAttendance.Core.Domain.Entities;
using ChurchAttendance.Core.Domain.Enums;
using ChurchAttendance.Core.Domain.RepositoryContracts.V1;
using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.Exceptions;
using ChurchAttendance.Core.ServiceContracts.V1;

namespace ChurchAttendance.Core.Services.V1;

public class EventService : IEventService
{
    private readonly IEventRepository _repo;
    private readonly IV1AttendanceDayRepository _dayRepo;
    private readonly IAuditService _audit;

    public EventService(IEventRepository repo, IV1AttendanceDayRepository dayRepo, IAuditService audit)
    {
        _repo = repo;
        _dayRepo = dayRepo;
        _audit = audit;
    }

    public async Task<List<SpecialEventDto>> GetAllAsync()
    {
        var events = await _repo.GetAllAsync();
        return events.Select(ToDto).ToList();
    }

    public async Task<SpecialEventDto> GetByIdAsync(Guid id)
    {
        var ev = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Event not found.");
        return ToDto(ev);
    }

    public async Task<SpecialEventDto> CreateAsync(CreateEventDto dto, string performedBy)
    {
        if (!Enum.TryParse<EventStatus>(dto.Status, true, out var status))
            throw new BadRequestException($"Invalid status '{dto.Status}'. Valid values: Published, Draft, Cancelled.");

        V1AttendanceDay? linkedDay = null;

        if (dto.CreateLinkedAttendanceDay)
        {
            linkedDay = new V1AttendanceDay
            {
                Id = Guid.NewGuid(),
                Title = dto.Title.Trim(),
                Date = DateOnly.FromDateTime(dto.StartAt.Date),
                StartAt = dto.StartAt,
                EndAt = dto.EndAt,
                Location = dto.Location?.Trim(),
                Status = AttendanceDayStatus.Scheduled,
                CreatedAtUtc = DateTime.UtcNow
            };
            await _dayRepo.AddAsync(linkedDay);
        }

        var ev = new SpecialEvent
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim(),
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Location = dto.Location?.Trim(),
            Status = status,
            LinkedAttendanceDayId = linkedDay?.Id,
            CreatedAtUtc = DateTime.UtcNow
        };

        var added = await _repo.AddAsync(ev);
        if (linkedDay is not null) added.LinkedAttendanceDay = linkedDay;

        await _audit.LogAsync("إنشاء حدث", nameof(SpecialEvent), added.Id.ToString(), $"تم إنشاء حدث «{added.Title}»", performedBy);
        return ToDto(added);
    }

    public async Task<SpecialEventDto> UpdateAsync(Guid id, UpdateEventDto dto, string performedBy)
    {
        var ev = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Event not found.");

        if (dto.Title is not null) ev.Title = dto.Title.Trim();
        if (dto.Description is not null) ev.Description = dto.Description.Trim();
        if (dto.StartAt.HasValue) ev.StartAt = dto.StartAt.Value;
        if (dto.EndAt.HasValue) ev.EndAt = dto.EndAt.Value;
        if (dto.Location is not null) ev.Location = dto.Location.Trim();
        if (dto.Status is not null)
        {
            if (!Enum.TryParse<EventStatus>(dto.Status, true, out var newStatus))
                throw new BadRequestException($"Invalid status '{dto.Status}'.");
            ev.Status = newStatus;
        }

        await _repo.UpdateAsync(ev);
        return ToDto(ev);
    }

    public async Task<SpecialEventDto> PatchStatusAsync(Guid id, string status, string performedBy)
    {
        if (!Enum.TryParse<EventStatus>(status, true, out var newStatus))
            throw new BadRequestException($"Invalid status '{status}'. Valid values: Published, Draft, Cancelled.");

        var ev = await _repo.GetByIdAsync(id)
            ?? throw new NotFoundException("Event not found.");

        ev.Status = newStatus;
        await _repo.UpdateAsync(ev);
        await _audit.LogAsync("تغيير حالة الحدث", nameof(SpecialEvent), id.ToString(), $"الحالة الجديدة: {status}", performedBy);
        return ToDto(ev);
    }

    private static SpecialEventDto ToDto(SpecialEvent e) => new()
    {
        Id = e.Id.ToString(),
        Title = e.Title,
        Description = e.Description,
        StartAt = e.StartAt,
        EndAt = e.EndAt,
        Location = e.Location,
        ImageUrl = e.ImageUrl,
        Status = e.Status.ToString(),
        LinkedAttendanceDayId = e.LinkedAttendanceDayId?.ToString()
    };
}
