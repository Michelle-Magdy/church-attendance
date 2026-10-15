using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a => a.AttendanceId);
            builder.Property(a => a.AttendanceId).ValueGeneratedNever();

            builder.Property(a => a.UserId).IsRequired();
            builder.Property(a => a.AttendanceDayId).IsRequired();
            builder.Property(a => a.MarkedAt).IsRequired();
            builder.Property(a => a.Method).HasConversion<int>().IsRequired();
            // Defines unique index across UserId and AttendanceDayId
            builder.HasIndex(a => new { a.UserId, a.AttendanceDayId })
                .IsUnique();

            builder.HasOne(a => a.User)
                   .WithMany(u => u.Attendances)
                   .HasForeignKey(a => a.UserId);

            builder.HasOne(a => a.AttendanceDay)
                   .WithMany()
                   .HasForeignKey(a => a.AttendanceDayId);
        }
    }
}
