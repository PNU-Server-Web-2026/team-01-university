namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// Кафедра університету.
/// </summary>
public class Department
{
    /// <summary>
    /// Унікальний ідентифікатор кафедри.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Повна назва кафедри.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Короткий код кафедри (2-10 великих латинських літер, напр. CS).
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Назва факультету.
    /// </summary>
    public string? Faculty { get; set; }
}
