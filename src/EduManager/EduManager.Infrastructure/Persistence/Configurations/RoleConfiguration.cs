using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");
    }
}