using Health.Domain.Entities.AppointmentModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.AppointmentConfigurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
           // Doctor Relationship
           builder.HasOne(a => a.DoctorProfile)
                  .WithMany(d => d.Appointments)
                  .HasForeignKey(a => a.DoctorProfileId)
                  .OnDelete(DeleteBehavior.Restrict);

           //Patient Relationship
           builder.HasOne(a => a.PatientProfile)
                  .WithMany(p => p.Appointments)
                  .HasForeignKey(a => a.PatientProfileId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Slot Relationship
            builder.HasOne(a => a.DoctorGeneratedSlots)
                   .WithOne()
                   .HasForeignKey<Appointment>(a => a.DoctorGeneratedSlotsId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes Performance
            builder.HasIndex(a => a.DoctorProfileId);
            builder.HasIndex(a => a.PatientProfileId);
            builder.HasIndex(a => a.DoctorGeneratedSlotsId);
        }
    }
}
