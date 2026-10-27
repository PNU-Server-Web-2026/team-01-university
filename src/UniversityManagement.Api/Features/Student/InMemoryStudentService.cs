using System.Collections.Concurrent; 

namespace UniversityManagement.Api.Features.Students; 

public class InMemoryStudentService : IStudentService
{
    
    private readonly ConcurrentDictionary<int, Student> _students = new();
    
     private int _currentId = 0; 

    public Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken cancellationToken = default) 
    {
        
        var response = _students.Values.Select(MapToResponse);
        return Task.FromResult(response.AsEnumerable()); 
    }

    public Task<StudentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default) 
    {
        
        var response = _students.TryGetValue(id, out var student) ? MapToResponse(student) : null;
        return Task.FromResult(response); 
    }

    public Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default) 
    {
        
        var newId = Interlocked.Increment(ref _currentId); 
        
        
        var student = new Student 
        {
            Id = newId, 
            FirstName = request.FirstName,
            LastName = request.LastName,   
            Email = request.Email,         
            RecordBookNumber = request.RecordBookNumber, 
            BirthDate = request.BirthDate  
        };

        
        _students.TryAdd(newId, student);
        
        return Task.FromResult(MapToResponse(student));
    }

    public Task<bool> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default) 
    {
        
        if (!_students.TryGetValue(id, out var existingStudent)) 
            return Task.FromResult(false); 

        
        existingStudent.FirstName = request.FirstName; 
        existingStudent.LastName = request.LastName;
        existingStudent.Email = request.Email;
        existingStudent.RecordBookNumber = request.RecordBookNumber;
        existingStudent.BirthDate = request.BirthDate;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) 
    {
       
        var isDeleted = _students.TryRemove(id, out _);
        return Task.FromResult(isDeleted); 
    }

    
    private static StudentResponse MapToResponse(Student student) => 
        new(student.Id, student.FirstName, student.LastName, student.Email, student.RecordBookNumber, student.BirthDate);
}