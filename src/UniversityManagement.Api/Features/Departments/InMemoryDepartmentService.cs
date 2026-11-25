using System.Collections.Concurrent;

namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// In-memory реалізація сервісу для роботи з кафедрами.
/// У S3 буде замінена на EF Core.
/// </summary>
public class InMemoryDepartmentService : IDepartmentService
{
    private readonly ConcurrentDictionary<int, Department> _departments = new();
    private int _nextId;

    public Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<DepartmentResponse> departments = _departments.Values
            .OrderBy(d => d.Id)
            .Select(ToResponse)
            .ToList();
        return Task.FromResult(departments);
    }

    public Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = _departments.TryGetValue(id, out var department) ? ToResponse(department) : null;
        return Task.FromResult(response);
    }

    public Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var department = new Department(id, request.Name, request.Code, request.Faculty);

        _departments[id] = department;
        return Task.FromResult(ToResponse(department));
    }

    public Task<bool> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        while (_departments.TryGetValue(id, out var current))
        {
            var updated = current with { Name = request.Name, Code = request.Code, Faculty = request.Faculty };
            if (_departments.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_departments.TryRemove(id, out _));
    }

    private static DepartmentResponse ToResponse(Department department) =>
        new(department.Id, department.Name, department.Code, department.Faculty);

    private sealed record Department(int Id, string Name, string Code, string Faculty);
}
