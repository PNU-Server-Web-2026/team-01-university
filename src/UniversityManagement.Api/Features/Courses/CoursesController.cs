using Microsoft.AspNetCore.Mvc;

namespace UniversityManagement.Api.Features.Courses;

/// <summary>
/// Контролер для управління навчальними курсами.
/// </summary>
[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// GET /api/courses — отримати всі курси.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var courses = await _courseService.GetAllAsync(cancellationToken);
        return Ok(courses);
    }

    /// <summary>
    /// GET /api/courses/{id} — отримати курс за ідентифікатором.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var course = await _courseService.GetByIdAsync(id, cancellationToken);
        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    /// <summary>
    /// POST /api/courses — створити новий курс.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseResponse>> CreateAsync(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var course = await _courseService.CreateAsync(request, cancellationToken);
        return Created($"/api/courses/{course.Id}", course);
    }

    /// <summary>
    /// PUT /api/courses/{id} — оновити курс.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CourseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseResponse>> UpdateAsync(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var course = await _courseService.UpdateAsync(id, request, cancellationToken);
        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    /// <summary>
    /// DELETE /api/courses/{id} — видалити курс.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await _courseService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
