using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagement.Api.Features.Teachers;

public static class TeachersFeatureExtensions
{
    public static IServiceCollection AddTeachersFeature(this IServiceCollection services)
    {
        services.AddSingleton<ITeacherService, InMemoryTeacherService>();
        return services;
    }
}
