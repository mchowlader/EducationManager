using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.EduTenants;

public class UserProfileConfiguration : EduEntityConfiguration, IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Mobile)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

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

        // Relationship
        builder.HasOne(x => x.User)
            .WithOne(x => x.Profile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasDatabaseName("IX_UserProfiles_UserId");
    }
}
