using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
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

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Mobile)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);

        builder.Property(x => x.RefreshTokenExpiry)
            .HasColumnType("timestamp with time zone");

        // Owned Entity — Address
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Division)
                .HasColumnName("Division")
                .HasMaxLength(100);

            address.Property(a => a.District)
                .HasColumnName("District")
                .HasMaxLength(100);

            address.Property(a => a.Thana)
                .HasColumnName("Thana")
                .HasMaxLength(100);

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(100);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(20);
        });

        // Indexes
        builder.HasIndex(x => x.UserCode)
            .IsUnique()
            .HasDatabaseName("IX_Users_UserCode");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");
    }
}