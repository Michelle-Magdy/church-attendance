using ChurchAttendance.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace ChurchAttendance.Infrastructure
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        // Legacy tables (keep for existing mobile app)
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<AttendanceDay> AttendanceDays { get; set; }

        // v1 tables for Ecclisiya frontend
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<V1AttendanceDay> V1AttendanceDays { get; set; }
        public virtual DbSet<V1Attendance> V1Attendances { get; set; }
        public virtual DbSet<SpecialEvent> SpecialEvents { get; set; }
        public virtual DbSet<ChurchSettings> ChurchSettings { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }

        public AppDbContext() { }   
        public AppDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ensure all DateTime properties are stored and read as UTC to avoid
            // timezone/Kind issues when using PostgreSQL (timestamp with time zone).
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));
                foreach (var prop in properties)
                {
                    if (prop.PropertyType == typeof(DateTime))
                    {
                        modelBuilder.Entity(entityType.Name).Property(prop.Name).HasConversion(dateTimeConverter);
                    }
                    else
                    {
                        modelBuilder.Entity(entityType.Name).Property(prop.Name).HasConversion(nullableDateTimeConverter);
                    }
                }
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
