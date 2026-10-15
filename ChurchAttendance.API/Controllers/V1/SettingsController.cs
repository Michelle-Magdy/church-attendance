using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _service;

    public SettingsController(ISettingsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ChurchSettingsDto>> Get()
        => Ok(await _service.GetAsync());

    [HttpPut]
    public async Task<ActionResult<ChurchSettingsDto>> Update([FromBody] ChurchSettingsDto dto)
        => Ok(await _service.UpdateAsync(dto));
}
