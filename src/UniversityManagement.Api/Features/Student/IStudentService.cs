namespace UniversityManagement.Api.Features.Students; 
public interface IStudentService 
{

    Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken cancellationToken = default); 
    
    Task<StudentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}