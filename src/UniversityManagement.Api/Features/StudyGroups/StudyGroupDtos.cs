namespace UniversityManagement.Api.Features.StudyGroups;

public record CreateStudyGroupRequest(
    string Name,
    int Year,
    string Specialty
);

public record UpdateStudyGroupRequest(
    string Name,
    int Year,
    string Specialty
);

public record StudyGroupResponse(
    int Id,
    string Name,
    int Year,
    string Specialty
);