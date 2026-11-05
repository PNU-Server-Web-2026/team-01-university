using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;
// Перевірте і вкажіть ваш реальний namespace з AppDbContext
using UniversityManagement.Api.Data; 

namespace UniversityManagement.Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("university_test_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string GetConnectionString() => _dbContainer.GetConnectionString();

    // Запускається xUnit один раз ПЕРЕД усіма тестами у колекції
    public async Task InitializeAsync()
    {
        // 1. Стартуємо контейнер PostgreSQL у Docker
        await _dbContainer.StartAsync();

        // 2. Крок 3 завдання: Створюємо scope і застосовуємо міграції EF Core
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Видаляємо оригінальну реєстрацію DbContext Options з Program.cs
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Перевизначаємо DbContext на використання рядка підключення з Testcontainers
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });
        });
    }

    // Запускається xUnit один раз ПІСЛЯ всіх тестів у колекції
    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}