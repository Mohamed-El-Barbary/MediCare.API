using Health.Domain.Entities.ReviewModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.ReviewConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Patient
            builder.HasOne(r => r.Patient)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Doctor
            builder.HasOne(r => r.doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.doctorId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Appointment
            builder.HasOne(r => r.Appointment)
                   .WithOne(a => a.Review)
                   .HasForeignKey<Review>(r => r.AppointmentId)
                   .OnDelete(DeleteBehavior.Cascade);
            // Indexes
            builder.HasIndex(r => r.AppointmentId)
                   .IsUnique();
            builder.HasIndex(r => r.doctorId);
            builder.HasIndex(r => r.PatientId);
        }
    }
}
