using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class SpecialEventConfiguration : IEntityTypeConfiguration<SpecialEvent>
{
    public void Configure(EntityTypeBuilder<SpecialEvent> builder)
    {
        builder.ToTable("SpecialEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Location).HasMaxLength(500);
        builder.Property(e => e.ImageUrl).HasMaxLength(1000);

        builder.HasOne(e => e.LinkedAttendanceDay)
               .WithMany()
               .HasForeignKey(e => e.LinkedAttendanceDayId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
