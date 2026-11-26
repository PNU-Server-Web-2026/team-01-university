using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UniversityManagement.Api.Features.StudyGroups;

[ApiController]
[Route("api/study-groups")]
public class StudyGroupsController : ControllerBase
{
    private readonly IStudyGroupService _service;

    public StudyGroupsController(IStudyGroupService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<StudyGroupResponse>>> GetAll(CancellationToken ct)
    {
        var groups = await _service.GetAllAsync(ct);
        return Ok(groups);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudyGroupResponse>> GetById(int id, CancellationToken ct)
    {
        var group = await _service.GetByIdAsync(id, ct);
        if (group == null)
        {
            return NotFound();
        }

        return Ok(group);
    }

    [HttpPost]
    public async Task<ActionResult<StudyGroupResponse>> Create(CreateStudyGroupRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateStudyGroupRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(id, request, ct);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}