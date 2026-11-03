using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagement.Api.Features.Courses;

/// <summary>
/// Extension методи для реєстрації сервісів курсів.
/// </summary>
public static class CoursesFeatureExtensions
{
    /// <summary>
    /// Реєструє сервіси для роботи з курсами.
    /// </summary>
    public static IServiceCollection AddCoursesFeature(this IServiceCollection services)
    {
        services.AddSingleton<ICourseService, InMemoryCourseService>();
        return services;
    }
}
