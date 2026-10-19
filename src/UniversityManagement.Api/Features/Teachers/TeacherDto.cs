namespace UniversityManagement.Api.Features.Teachers;

public enum AcademicTitle
{
    Assistant,
    SeniorLecturer,
    AssociateProfessor,
    Professor
}

public record TeacherResponse(int Id, string FirstName, string LastName, string Email, AcademicTitle AcademicTitle);

public record CreateTeacherRequest(string FirstName, string LastName, string Email, AcademicTitle AcademicTitle);

public record UpdateTeacherRequest(string FirstName, string LastName, string Email, AcademicTitle AcademicTitle);
