using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/events")]
public class EventsController : ControllerBase
{
    private readonly IEventService _service;

    public EventsController(IEventService service)
    {
        _service = service;
    }

    private string PerformedBy => User.Identity?.Name ?? User.FindFirst("sub")?.Value ?? "unknown";

    [HttpGet]
    public async Task<ActionResult<List<SpecialEventDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SpecialEventDto>> Get(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<SpecialEventDto>> Create([FromBody] CreateEventDto dto)
    {
        var created = await _service.CreateAsync(dto, PerformedBy);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SpecialEventDto>> Update(Guid id, [FromBody] UpdateEventDto dto)
        => Ok(await _service.UpdateAsync(id, dto, PerformedBy));

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<SpecialEventDto>> PatchStatus(
        Guid id, [FromBody] PatchEventStatusDto dto)
        => Ok(await _service.PatchStatusAsync(id, dto.Status, PerformedBy));
}
