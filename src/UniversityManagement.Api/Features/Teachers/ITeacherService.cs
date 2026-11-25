namespace UniversityManagement.Api.Features.Teachers;

public interface ITeacherService
{
    Task<IEnumerable<TeacherResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TeacherResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TeacherResponse> CreateAsync(CreateTeacherRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, UpdateTeacherRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
