using Microsoft.EntityFrameworkCore;
using UniversityManagement.Api.Features.Departments;

namespace UniversityManagement.Api.Data;

/// <summary>
/// Контекст бази даних застосунку.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Кафедри університету.
    /// </summary>
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Підхоплює всі класи IEntityTypeConfiguration<T> з цієї збірки.
        // Конфігурацію сутності тримайте поруч із фічею: Features/<Name>/<Entity>Configuration.cs
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
