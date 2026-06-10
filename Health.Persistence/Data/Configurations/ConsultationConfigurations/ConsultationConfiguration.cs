using Health.Domain.Entities.ConsultationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Health.Persistence.Data.Configurations.ConsultationConfigurations
{
    internal class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> builder)
        {

            builder.ToTable("Consultations");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.RoomId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Diagnosis)
                .HasMaxLength(2000);

            builder.Property(c => c.Symptoms)
                .HasMaxLength(2000);

            builder.HasOne(c => c.Doctor)
                .WithMany()
                .HasForeignKey(c => c.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Connections)
                .WithOne(sc => sc.Consultation)
                .HasForeignKey(sc => sc.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.RoomId)
                .IsUnique();

            builder.HasIndex(c => c.AppointmentId)
                .IsUnique();

        }
    }
}
