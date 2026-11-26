using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UniversityManagement.Api.Features.Teachers;

/// <summary>
/// Конфігурація EF Core для сутності <see cref="Teacher"/>.
/// </summary>
public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(t => t.Email)
            .IsUnique();

        builder.Property(t => t.AcademicTitle)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
