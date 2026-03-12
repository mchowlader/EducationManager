using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.EduTenants;

public class ClassesConfiguration : EduEntityConfiguration, IEntityTypeConfiguration<Classes>
{
    public void Configure(EntityTypeBuilder<Classes> builder)
    {
        builder.ToTable("Classes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Classes_Name");
    }
}