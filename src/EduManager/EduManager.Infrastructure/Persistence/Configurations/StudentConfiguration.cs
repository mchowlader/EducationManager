using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.StudentCode)
            .HasMaxLength(20)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Section)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Student>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.StudentCode)
            .IsUnique()
            .HasDatabaseName("IX_Students_StudentCode");

        builder.HasIndex(x => new { x.SectionId, x.ClassRoll })
            .IsUnique()
            .HasDatabaseName("IX_Students_SectionId_ClassRoll");
    }
}