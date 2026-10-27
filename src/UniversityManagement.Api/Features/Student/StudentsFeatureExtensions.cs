using Microsoft.Extensions.DependencyInjection; 

namespace UniversityManagement.Api.Features.Students; 

public static class StudentsFeatureExtensions 
{
    public static IServiceCollection AddStudentsFeature(this IServiceCollection services) 
    {
        
        services.AddSingleton<IStudentService, InMemoryStudentService>();
        
        return services;
    }
}