using Microsoft.AspNetCore.Mvc; 

namespace UniversityManagement.Api.Features.Students; 

[ApiController] 
[Route("api/students")]
public class StudentsController : ControllerBase 
{
    private readonly IStudentService _studentService; 

    public StudentsController(IStudentService studentService) 
    {
        _studentService = studentService; 
    }

    [HttpGet] 
    public async Task<ActionResult<IEnumerable<StudentResponse>>> GetAll(CancellationToken cancellationToken)
    {
        
        var students = await _studentService.GetAllAsync(cancellationToken);
        return Ok(students); 
    }

    [HttpGet("{id}")] 
    public async Task<ActionResult<StudentResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        
        var student = await _studentService.GetByIdAsync(id, cancellationToken);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost] 
    public async Task<ActionResult<StudentResponse>> Create(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var createdStudent = await _studentService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, createdStudent);
    }

    [HttpPut("{id}")] 
    public async Task<IActionResult> Update(int id, UpdateStudentRequest request, CancellationToken cancellationToken)
    {
        var isUpdated = await _studentService.UpdateAsync(id, request, cancellationToken);
        return isUpdated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")] 
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _studentService.DeleteAsync(id, cancellationToken);
        return isDeleted ? NoContent() : NotFound();
    }
}