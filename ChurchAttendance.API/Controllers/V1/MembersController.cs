using ChurchAttendance.Core.DTOs.V1;
using ChurchAttendance.Core.ServiceContracts.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChurchAttendance.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    /// <summary>GET /api/v1/members — paginated, filterable member list.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMembers([FromQuery] MemberListQueryDto query)
    {
        var result = await _memberService.GetPagedAsync(query);
        return Ok(result);
    }

    /// <summary>GET /api/v1/members/{id}</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetMember(Guid id)
        => Ok(await _memberService.GetByIdAsync(id));

    /// <summary>POST /api/v1/members — create a member.</summary>
    [HttpPost]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto dto)
    {
        var created = await _memberService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetMember), new { id = created.Id }, created);
    }

    /// <summary>PUT /api/v1/members/{id} — update editable fields (phone is read-only).</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MemberDto>> UpdateMember(Guid id, [FromBody] UpdateMemberDto dto)
        => Ok(await _memberService.UpdateAsync(id, dto));

    /// <summary>PATCH /api/v1/members/{id}/status — approve, suspend, or set status.</summary>
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<MemberDto>> PatchStatus(Guid id, [FromBody] PatchMemberStatusDto dto)
        => Ok(await _memberService.PatchStatusAsync(id, dto.Status));

    /// <summary>DELETE /api/v1/members/{id} — soft-delete.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMember(Guid id)
    {
        await _memberService.SoftDeleteAsync(id);
        return NoContent();
    }
}
