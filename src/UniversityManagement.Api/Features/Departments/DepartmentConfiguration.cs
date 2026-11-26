using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UniversityManagement.Api.Features.Departments;

/// <summary>
/// Конфігурація EF Core для сутності <see cref="Department"/>.
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments", t =>
        {
            // Обмеження формату Code на рівні PostgreSQL: 2-10 великих латинських літер
            t.HasCheckConstraint("CK_Departments_Code_Format", "\"Code\" ~ '^[A-Z]{2,10}$'");
        });

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.Property(d => d.Faculty)
            .HasMaxLength(200);
    }
}
