using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Api.Features.Courses;

/// <summary>
/// DTO для створення нового курсу.
/// </summary>
/// <param name="Title">Назва курсу (обов'язкове, до 200 символів)</param>
/// <param name="Code">Код курсу (наприклад, CS-301)</param>
/// <param name="Credits">Кредити ЄКТС 1–15</param>
/// <param name="Capacity">Максимум студентів 1–300</param>
/// <param name="Semester">Семестр 1–8</param>
public record CreateCourseRequest(
    [Required]
    [StringLength(200)]
    string Title,

    [Required]
    [StringLength(50)]
    string Code,

    [Required]
    [Range(1, 15, ErrorMessage = "Credits must be between 1 and 15")]
    int Credits,

    [Required]
    [Range(1, 300, ErrorMessage = "Capacity must be between 1 and 300")]
    int Capacity,

    [Required]
    [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8")]
    int Semester
);

/// <summary>
/// DTO для оновлення існуючого курсу.
/// </summary>
/// <param name="Title">Назва курсу (обов'язкове, до 200 символів)</param>
/// <param name="Code">Код курсу (наприклад, CS-301)</param>
/// <param name="Credits">Кредити ЄКТС 1–15</param>
/// <param name="Capacity">Максимум студентів 1–300</param>
/// <param name="Semester">Семестр 1–8</param>
public record UpdateCourseRequest(
    [Required]
    [StringLength(200)]
    string Title,

    [Required]
    [StringLength(50)]
    string Code,

    [Required]
    [Range(1, 15, ErrorMessage = "Credits must be between 1 and 15")]
    int Credits,

    [Required]
    [Range(1, 300, ErrorMessage = "Capacity must be between 1 and 300")]
    int Capacity,

    [Required]
    [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8")]
    int Semester
);

/// <summary>
/// DTO для відображення інформації про курс.
/// </summary>
/// <param name="Id">Унікальний ідентифікатор курсу</param>
/// <param name="Title">Назва курсу</param>
/// <param name="Code">Код курсу</param>
/// <param name="Credits">Кредити ЄКТС</param>
/// <param name="Capacity">Максимум студентів</param>
/// <param name="Semester">Семестр</param>
public record CourseResponse(
    int Id,
    string Title,
    string Code,
    int Credits,
    int Capacity,
    int Semester
);
