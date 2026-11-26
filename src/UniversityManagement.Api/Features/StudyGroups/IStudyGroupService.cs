using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UniversityManagement.Api.Features.StudyGroups;

public interface IStudyGroupService
{
    Task<IReadOnlyCollection<StudyGroupResponse>> GetAllAsync(CancellationToken ct = default);
    Task<StudyGroupResponse?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<StudyGroupResponse> CreateAsync(CreateStudyGroupRequest request, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateStudyGroupRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}