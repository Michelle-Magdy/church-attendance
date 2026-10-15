using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations
{
    public class AttendanceDayConfiguration : IEntityTypeConfiguration<AttendanceDay>
    {
        public void Configure(EntityTypeBuilder<AttendanceDay> builder)
        {
            builder.ToTable("AttendanceDays");

            builder.HasKey(ad => ad.AttendanceDayId);
            builder.Property(ad => ad.AttendanceDayId).ValueGeneratedNever();

            builder.Property(ad => ad.Title)
                   .HasMaxLength(200);

            builder.Property(ad => ad.QrCode)
                   .HasMaxLength(500);

            builder.Property(ad => ad.Date).IsRequired();
            builder.Property(ad => ad.IsOpen).IsRequired();
            builder.Property(ad => ad.CreatedAt).IsRequired();
        }
    }
}
