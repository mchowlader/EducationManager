using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.EduTenants;

public class SectionConfiguration : EduEntityConfiguration, IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Name)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Class)
            .WithMany(x => x.Sections)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => new { x.ClassId, x.Name })
            .IsUnique()
            .HasDatabaseName("IX_Sections_ClassId_Name");
    }
}