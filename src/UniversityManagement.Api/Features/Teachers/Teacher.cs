namespace UniversityManagement.Api.Features.Teachers;

/// <summary>
/// Викладач університету.
/// </summary>
public class Teacher
{
    /// <summary>
    /// Унікальний ідентифікатор викладача.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Ім'я викладача.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Прізвище викладача.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Електронна пошта викладача.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Вчене звання або посада викладача.
    /// </summary>
    public AcademicTitle AcademicTitle { get; set; }
}
