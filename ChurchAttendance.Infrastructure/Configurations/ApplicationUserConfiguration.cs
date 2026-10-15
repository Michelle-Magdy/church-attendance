using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Keep default Identity mappings; configure additional properties
            builder.Property(u => u.PersonName)
                   .HasMaxLength(200);

            builder.Property(u => u.Confessor)
                   .HasMaxLength(200);

            builder.Property(u => u.Address)
                   .HasMaxLength(500);

            builder.Property(u => u.ChurchCode)
                   .HasMaxLength(100);

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt).IsRequired(false);
            builder.Property(u => u.isActive).HasDefaultValue(true);
        }
    }
}
