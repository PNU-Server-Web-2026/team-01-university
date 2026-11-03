using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UniversityManagement.Api.Features.Courses;

public class InMemoryCourseService : ICourseService
{
    private readonly ConcurrentDictionary<int, CourseResponse> _courses = new();
    private int _nextId = 1;

    public Task<CourseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _courses.TryGetValue(id, out var course);
        return Task.FromResult(course);
    }

    public Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<CourseResponse>>(_courses.Values.ToList());
    }

    public Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var course = new CourseResponse(id, request.Title, request.Code, request.Credits, request.Capacity, request.Semester);

        _courses[id] = course;
        return Task.FromResult(course);
    }

    public Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_courses.TryGetValue(id, out var existing))
        {
            return Task.FromResult<CourseResponse?>(null);
        }

        var updated = existing with
        {
            Title = request.Title,
            Code = request.Code,
            Credits = request.Credits,
            Capacity = request.Capacity,
            Semester = request.Semester
        };

        _courses[id] = updated;
        return Task.FromResult<CourseResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = _courses.TryRemove(id, out _);
        return Task.FromResult(deleted);
    }

    public void Clear()
    {
        _courses.Clear();
        _nextId = 1;
    }
}
