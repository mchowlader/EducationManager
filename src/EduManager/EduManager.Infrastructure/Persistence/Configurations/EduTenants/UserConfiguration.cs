using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.EduTenants;

public class UserConfiguration : EduEntityConfiguration, IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.UserCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);

        builder.Property(x => x.RefreshTokenExpiry)
            .HasColumnType("timestamp with time zone");

        // Indexes
        builder.HasIndex(x => x.UserCode)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserCode");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");
    }
}