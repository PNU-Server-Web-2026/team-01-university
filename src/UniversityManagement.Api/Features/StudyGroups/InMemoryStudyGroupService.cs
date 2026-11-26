using System.Collections.Concurrent;

namespace UniversityManagement.Api.Features.StudyGroups;

public class InMemoryStudyGroupService : IStudyGroupService
{
    private readonly ConcurrentDictionary<int, StudyGroupResponse> _store = new();
    private int _idCounter = 0;

    public Task<IReadOnlyCollection<StudyGroupResponse>> GetAllAsync(CancellationToken ct = default)
    {
        IReadOnlyCollection<StudyGroupResponse> result = _store.Values.ToList().AsReadOnly();
        return Task.FromResult(result);
    }

    public Task<StudyGroupResponse?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var group);
        return Task.FromResult(group);
    }

    public Task<StudyGroupResponse> CreateAsync(CreateStudyGroupRequest request, CancellationToken ct = default)
    {
        var newId = Interlocked.Increment(ref _idCounter);
        var response = new StudyGroupResponse(newId, request.Name, request.Year, request.Specialty);

        _store[newId] = response;
        return Task.FromResult(response);
    }

    public Task<bool> UpdateAsync(int id, UpdateStudyGroupRequest request, CancellationToken ct = default)
    {
        while (_store.TryGetValue(id, out var existing))
        {
            var updatedResponse = new StudyGroupResponse(id, request.Name, request.Year, request.Specialty);
            if (_store.TryUpdate(id, updatedResponse, existing))
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var removed = _store.TryRemove(id, out _);
        return Task.FromResult(removed);
    }
}