using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();
        builder.Property(l => l.Action).IsRequired().HasMaxLength(200);
        builder.Property(l => l.TargetType).IsRequired().HasMaxLength(100);
        builder.Property(l => l.TargetId).HasMaxLength(100);
        builder.Property(l => l.Details).HasMaxLength(1000);
        builder.Property(l => l.PerformedBy).IsRequired().HasMaxLength(200);

        builder.HasIndex(l => l.Timestamp);
    }
}
