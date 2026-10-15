using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class V1AttendanceDayConfiguration : IEntityTypeConfiguration<V1AttendanceDay>
{
    public void Configure(EntityTypeBuilder<V1AttendanceDay> builder)
    {
        builder.ToTable("V1AttendanceDays");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();
        builder.Property(d => d.Title).IsRequired().HasMaxLength(300);
        builder.Property(d => d.Location).HasMaxLength(500);
        builder.Property(d => d.Notes).HasMaxLength(2000);
        builder.Property(d => d.QrToken).HasMaxLength(200);

        // Ignore computed property
        builder.Ignore(d => d.IsOpen);
    }
}
