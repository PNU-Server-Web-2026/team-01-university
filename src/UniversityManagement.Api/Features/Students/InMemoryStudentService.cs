using System.Collections.Concurrent;

namespace UniversityManagement.Api.Features.Students;

public sealed class InMemoryStudentService : IStudentService
{
    // Внутрішній незмінний record замість мутабельного класу
    private sealed record StudentRecord(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        string RecordBookNumber,
        DateOnly BirthDate)
    {
        public StudentResponse ToResponse() =>
            new(Id, FirstName, LastName, Email, RecordBookNumber, BirthDate );
    }

    private readonly ConcurrentDictionary<int, StudentRecord> _students = new();
    private int _nextId;

   public Task<IEnumerable<StudentResponse>> GetAllAsync(CancellationToken ct = default)
    {
        IEnumerable<StudentResponse> result = _students.Values
            .Select(s => s.ToResponse())
            .ToList();

        return Task.FromResult(result);
    }

    public Task<StudentResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(
            _students.TryGetValue(id, out var student) ? student.ToResponse() : null);
    }

    public Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken ct = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var record = new StudentRecord(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.RecordBookNumber,
            request.BirthDate );

        _students[id] = record;
        return Task.FromResult(record.ToResponse());
    }

    public Task<bool> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            // 1. Отримуємо поточний знімок запису (якщо видалили — повертаємо false)
            if (!_students.TryGetValue(id, out var current))
            {
                return Task.FromResult(false);
            }

            // 2. Створюємо новий незмінний екземпляр
            var updated = new StudentRecord(
                id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.RecordBookNumber,
                request.BirthDate );

            // 3. Атомарно замінюємо лише якщо значення в словнику все ще дорівнює current
            if (_students.TryUpdate(id, updated, current))
            {
                return Task.FromResult(true);
            }
            // Якщо інший потік встиг змінити запис між TryGetValue та TryUpdate — цикл повторюється
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return Task.FromResult(_students.TryRemove(id, out _));
    }
}