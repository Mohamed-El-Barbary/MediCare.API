using Health.Domain.Entities.ConsultationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.ConsultationConfigurations
{
    internal class PrescriptionConfigurations : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.AdditionalNotes)
                .HasMaxLength(2000);

            builder.HasOne(p => p.Consultation)
                .WithMany(c => c.Prescriptions)
                .HasForeignKey(p => p.ConsultationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.ConsultationId);
        }
    }
}
