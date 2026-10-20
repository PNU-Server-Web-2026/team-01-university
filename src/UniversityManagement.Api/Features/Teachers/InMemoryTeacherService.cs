using System.Collections.Concurrent;

namespace UniversityManagement.Api.Features.Teachers;

public class InMemoryTeacherService : ITeacherService
{
    private readonly ConcurrentDictionary<int, TeacherResponse> _teachers = new();

    private int _currentId = 0;

    public Task<IEnumerable<TeacherResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<TeacherResponse>>(_teachers.Values);
    }

    public Task<TeacherResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _teachers.TryGetValue(id, out var teacher);
        return Task.FromResult(teacher);
    }

    public Task<TeacherResponse> CreateAsync(CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _currentId);

        var teacher = new TeacherResponse(id, request.FirstName, request.LastName, request.Email, request.AcademicTitle);
        _teachers.TryAdd(id, teacher);

        return Task.FromResult(teacher);
    }

    public Task<bool> UpdateAsync(int id, UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        if (!_teachers.ContainsKey(id))
            return Task.FromResult(false);

        var updatedTeacher = new TeacherResponse(id, request.FirstName, request.LastName, request.Email, request.AcademicTitle);
        _teachers[id] = updatedTeacher;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_teachers.TryRemove(id, out _));
    }
}
