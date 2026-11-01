namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// Сервіс для роботи з кафедрами.
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// Отримати всі кафедри.
    /// </summary>
    Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати кафедру за ідентифікатором.
    /// </summary>
    Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Створити нову кафедру.
    /// </summary>
    Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновити існуючу кафедру.
    /// </summary>
    Task<DepartmentResponse?> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видалити кафедру.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистити всі дані (тільки для тестів).
    /// </summary>
    void Clear();
}
