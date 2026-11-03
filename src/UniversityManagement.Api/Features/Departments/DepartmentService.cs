using System.Collections.Concurrent;
using System.Threading;

namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// In-memory реалізація сервісу для роботи з кафедрами.
/// У S3 буде замінена на EF Core.
/// </summary>
public class InMemoryDepartmentService : IDepartmentService
{
    private readonly ConcurrentDictionary<int, DepartmentResponse> _departments = new();
    private int _nextId = 0;

    public Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<DepartmentResponse>>(_departments.Values);
    }

    public Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _departments.TryGetValue(id, out var department);
        return Task.FromResult(department);
    }

    public Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var department = new DepartmentResponse(
            id,
            request.Name,
            request.Code,
            request.Faculty
        );

        _departments.TryAdd(id, department);
        return Task.FromResult(department);
    }

    public Task<DepartmentResponse?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        if (!_departments.ContainsKey(id))
        {
            return Task.FromResult<DepartmentResponse?>(null);
        }

        var updated = new DepartmentResponse(
            id,
            request.Name,
            request.Code,
            request.Faculty
        );

        _departments[id] = updated;
        return Task.FromResult<DepartmentResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var removed = _departments.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    public void Clear()
    {
        _departments.Clear();
        _nextId = 0;
    }
}
