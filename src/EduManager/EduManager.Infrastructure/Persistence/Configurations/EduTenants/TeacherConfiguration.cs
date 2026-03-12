using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.EduTenants;

public class TeacherConfiguration : EduEntityConfiguration, IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.TeacherCode)
            .HasMaxLength(20)
            .ValueGeneratedOnAdd();


        builder.Property(x => x.Designation)
            .HasMaxLength(100)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Teacher>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.TeacherCode)
            .IsUnique()
            .HasDatabaseName("IX_Teachers_TeacherCode");
    }
}