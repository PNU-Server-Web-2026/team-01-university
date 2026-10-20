using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Api.Common;

public sealed class UniversityOptions
{
    public const string SectionName = "University";

    [Range(1, 60, ErrorMessage = "MaxCreditsPerSemester must be between 1 and 60.")]
    public int MaxCreditsPerSemester { get; init; } = 30;

    [Range(0, 100, ErrorMessage = "PassingGrade must be between 0 and 100.")]
    public int PassingGrade { get; init; } = 60;

    [Required(ErrorMessage = "EnrollmentDeadline is required.")]
    public DateTimeOffset? EnrollmentDeadline { get; init; }
}