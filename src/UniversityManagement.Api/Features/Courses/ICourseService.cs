namespace UniversityManagement.Api.Features.Courses;

/// <summary>
/// Сервіс для роботи з курсами.
/// </summary>
public interface ICourseService
{
    /// <summary>
    /// Отримати всі курси.
    /// </summary>
    Task<IEnumerable<CourseResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отримати курс за ідентифікатором.
    /// </summary>
    Task<CourseResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Створити новий курс.
    /// </summary>
    Task<CourseResponse> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновити існуючий курс.
    /// </summary>
    Task<CourseResponse?> UpdateAsync(int id, UpdateCourseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видалити курс.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистити всі дані (тільки для тестів).
    /// </summary>
    void Clear();
}
