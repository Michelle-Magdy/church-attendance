using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class ChurchSettingsConfiguration : IEntityTypeConfiguration<ChurchSettings>
{
    public void Configure(EntityTypeBuilder<ChurchSettings> builder)
    {
        builder.ToTable("ChurchSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ChurchName).HasMaxLength(300);
        builder.Property(s => s.ContactPhone).HasMaxLength(30);
        builder.Property(s => s.ContactEmail).HasMaxLength(200);
        builder.Property(s => s.MaleContactWhatsAppUrl).HasMaxLength(500);
        builder.Property(s => s.MaleGroupWhatsAppUrl).HasMaxLength(500);
        builder.Property(s => s.FemaleGroupWhatsAppUrl).HasMaxLength(500);
        builder.Property(s => s.FacebookPageUrl).HasMaxLength(500);

        // Seed one default row
        builder.HasData(new ChurchSettings { Id = 1 });
    }
}
