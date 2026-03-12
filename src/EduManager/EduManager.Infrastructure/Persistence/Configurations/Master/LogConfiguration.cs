using EduManager.Domain.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduManager.Infrastructure.Persistence.Configurations.Master;

public class LogConfiguration : MasterEntityConfiguration, IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs");
        builder.HasKey(x => x.Id);

        builder.Property(l => l.Id)
            .UseIdentityByDefaultColumn();

        builder.Property(l => l.TimeStamp)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(l => l.Level)
            .HasMaxLength (50)
            .IsRequired ();

        builder.Property(x => x.Message)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(x => x.Exception)
            .HasColumnType("text");

        builder.Property(x => x.Properties)
           .HasColumnType("text");


        builder.Property(x => x.ErrorCode)
            .HasMaxLength(50);

        //Index
        builder.HasIndex(x => x.Id)
            .HasDatabaseName("IX_Logs_Id");

        builder.HasIndex(x => x.TimeStamp)
            .HasDatabaseName("IX_Logs_Timestamp");

        builder.HasIndex(x => x.Level)
           .HasDatabaseName("IX_Logs_Level");

        builder.HasIndex(x => x.ErrorCode)
            .HasDatabaseName("IX_Logs_ErrorCode");
    }
}
