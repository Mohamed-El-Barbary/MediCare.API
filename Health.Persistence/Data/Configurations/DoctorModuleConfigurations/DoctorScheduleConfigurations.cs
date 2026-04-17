using Health.Domain.Entities.DoctorModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.DoctorModuleConfigurations
{
    public class DoctorScheduleConfigurations : IEntityTypeConfiguration<DoctorSchedule>
    {
        public void Configure(EntityTypeBuilder<DoctorSchedule> builder)
        {
            builder.Property(s => s.DayOfWeek)
                   .IsRequired();

            builder.Property(s => s.StartTime)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(s => s.EndTime)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(s => s.SlotDurationMinutes)
                   .IsRequired();

            builder.HasOne(p => p.DoctorProfile)
                   .WithMany(d => d.DoctorSchedule)
                   .HasForeignKey(p => p.DoctorProfileId);
        }
    }
}
