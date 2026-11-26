using Microsoft.Extensions.DependencyInjection;

namespace UniversityManagement.Api.Features.StudyGroups;

public static class StudyGroupsFeatureExtensions
{
    public static IServiceCollection AddStudyGroupsFeature(this IServiceCollection services)
    {
        
        services.AddSingleton<IStudyGroupService, InMemoryStudyGroupService>();
        return services;
    }
}