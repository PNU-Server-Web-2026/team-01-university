using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using UniversityManagement.Api.Tests.Features.Logging;

namespace UniversityManagement.Api.Tests;

/// <summary>
/// Фабрика тестового сервера для інтеграційних тестів.
/// Запускає API в середовищі "Testing" без Docker і без реальної БД:
/// AppDbContext зареєстрований, але поки що не використовується.
/// Коли з'являться тести, які працюють із БД, тут можна підмінити
/// реєстрацію DbContext (наприклад, на Testcontainers або SQLite).
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Збирає всі події Serilog цього тестового хоста (через ReadFrom.Services).
    /// </summary>
    public CollectingSink LogSink { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
            services.AddSingleton<ILogEventSink>(LogSink));
    }
}
