using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations;

public class TenantConfiguration
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Mobile)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ConnectionString)
            .HasMaxLength(500)
            .IsRequired();

        #region Index
        builder.HasIndex(x => x.Id);
        builder.HasIndex(x => x.Slug)
            .IsUnique();
        builder.HasIndex(x => x.Email)
            .IsUnique();
        builder.HasIndex(x => x.Mobile)
            .IsUnique();
        #endregion
    }
}
