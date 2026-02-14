using Health.Domain.Entities.DoctorModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations
{
    public class DoctorGeneratedSlotsConfigurations : IEntityTypeConfiguration<DoctorGeneratedSlots>
    {
        public void Configure(EntityTypeBuilder<DoctorGeneratedSlots> builder)
        {
            builder.Property(s => s.SlotDate)
                   .IsRequired()
                   .HasColumnType("date"); 

            builder.Property(s => s.StartTime)
                   .IsRequired()
                   .HasColumnType("time"); 

            builder.Property(s => s.EndTime)
                   .IsRequired()
                   .HasColumnType("time");

            builder.Property(s => s.IsBooked)
                   .IsRequired();

            builder.HasOne(s => s.DoctorSchedule)
                   .WithMany(ds => ds.DoctorGeneratedSlots)
                   .HasForeignKey(s => s.DoctorScheduleId);

            builder.HasOne(s => s.DoctorProfile)
                   .WithMany(d => d.DoctorGeneratedSlots) 
                   .HasForeignKey(s => s.DoctorProfileId);
        }
    }
}
