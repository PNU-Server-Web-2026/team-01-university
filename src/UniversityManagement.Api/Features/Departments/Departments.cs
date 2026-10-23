using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// DTO для створення нової кафедри.
/// </summary>
/// <param name="Name">Назва кафедри (обов'язкове, до 200 символів)</param>
/// <param name="Code">Код кафедри: 2–10 великих латинських літер (наприклад, CS)</param>
/// <param name="Faculty">Факультет (до 200 символів)</param>
public record CreateDepartmentRequest(
    [Required]
    [StringLength(200)]
    string Name,

    [Required]
    [RegularExpression("^[A-Z]{2,10}$", ErrorMessage = "Code must be 2-10 uppercase Latin letters")]
    string Code,

    [StringLength(200)]
    string Faculty
);

/// <summary>
/// DTO для оновлення існуючої кафедри.
/// </summary>
/// <param name="Name">Назва кафедри (обов'язкове, до 200 символів)</param>
/// <param name="Code">Код кафедри: 2–10 великих латинських літер (наприклад, CS)</param>
/// <param name="Faculty">Факультет (до 200 символів)</param>
public record UpdateDepartmentRequest(
    [Required]
    [StringLength(200)]
    string Name,

    [Required]
    [RegularExpression("^[A-Z]{2,10}$", ErrorMessage = "Code must be 2-10 uppercase Latin letters")]
    string Code,

    [StringLength(200)]
    string Faculty
);

/// <summary>
/// DTO для відображення інформації про кафедру.
/// </summary>
/// <param name="Id">Унікальний ідентифікатор кафедри</param>
/// <param name="Name">Назва кафедри</param>
/// <param name="Code">Код кафедри: 2–10 великих латинських літер (наприклад, CS)</param>
/// <param name="Faculty">Факультет</param>
public record DepartmentResponse(
    int Id,
    string Name,
    string Code,
    string Faculty
);
