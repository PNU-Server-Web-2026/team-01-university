using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// Extension методи для реєстрації сервісів кафедр.
/// </summary>
public static class DepartmentsFeatureExtensions
{
    /// <summary>
    /// Реєструє сервіси для роботи з кафедрами.
    /// </summary>
    public static IServiceCollection AddDepartmentsFeature(this IServiceCollection services)
    {
        services.AddSingleton<IDepartmentService, InMemoryDepartmentService>();
        return services;
    }
}
