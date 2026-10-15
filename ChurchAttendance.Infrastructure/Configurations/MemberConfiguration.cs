using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(m => m.ConfessionFather).HasMaxLength(200);
        builder.Property(m => m.Address).HasMaxLength(500);
        builder.Property(m => m.LevelId).HasMaxLength(100);

        // Unique phone among non-deleted (enforced in service logic — partial index if needed)
        builder.HasIndex(m => m.PhoneNumber).IsUnique();

        builder.HasOne(m => m.Level)
               .WithMany()
               .HasForeignKey(m => m.LevelId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
