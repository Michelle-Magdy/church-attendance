using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class V1AttendanceConfiguration : IEntityTypeConfiguration<V1Attendance>
{
    public void Configure(EntityTypeBuilder<V1Attendance> builder)
    {
        builder.ToTable("V1Attendances");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.Notes).HasMaxLength(500);

        builder.HasOne(a => a.Member)
               .WithMany(m => m.Attendances)
               .HasForeignKey(a => a.MemberId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.AttendanceDay)
               .WithMany(d => d.Attendances)
               .HasForeignKey(a => a.AttendanceDayId)
               .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate attendance for same member/day
        builder.HasIndex(a => new { a.MemberId, a.AttendanceDayId }).IsUnique();
    }
}
