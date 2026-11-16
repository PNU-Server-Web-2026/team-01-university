using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace UniversityManagement.Api.Features.Cors;

public static class CorsFeatureExtensions
{
    public const string PolicyName = "FrontPolicy";

    public static IServiceCollection AddCorsFeature(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithHeaders("Content-Type", "Authorization")
                      .WithMethods("GET", "POST", "PUT", "DELETE");
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsFeature(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}