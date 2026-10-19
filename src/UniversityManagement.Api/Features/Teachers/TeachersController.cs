using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Api.Features.Teachers;

[ApiController]
[Route("api/teachers")] 
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var teachers = await _teacherService.GetAllAsync(cancellationToken);
        return Ok(teachers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);
        if (teacher == null)
            return NotFound(); // Повертає 404, якщо не знайдено

        return Ok(teacher);
    }

    [HttpPost]
    public async Task<ActionResult<TeacherResponse>> Create([FromBody] CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        var teacher = await _teacherService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        var updated = await _teacherService.UpdateAsync(id, request, cancellationToken);
        if (!updated)
            return NotFound();

        return NoContent(); 
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _teacherService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
