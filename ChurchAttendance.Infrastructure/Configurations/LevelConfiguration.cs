using ChurchAttendance.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChurchAttendance.Infrastructure.Configurations;

public class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ToTable("Levels");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasMaxLength(100).ValueGeneratedNever();
        builder.Property(l => l.Name).IsRequired().HasMaxLength(200);

        // Seed default levels
        builder.HasData(
            new Level { Id = "level-children", Name = "أطفال", SortOrder = 1 },
            new Level { Id = "level-preparatory", Name = "إعدادي", SortOrder = 2 },
            new Level { Id = "level-secondary", Name = "ثانوي", SortOrder = 3 },
            new Level { Id = "level-university", Name = "جامعة", SortOrder = 4 },
            new Level { Id = "level-servants", Name = "خدام", SortOrder = 5 },
            new Level { Id = "level-families", Name = "أسر", SortOrder = 6 }
        );
    }
}
