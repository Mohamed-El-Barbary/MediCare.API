using Health.Domain.Entities.ConsultationModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.ConsultationConfigurations
{
    internal class PrescriptionItemConfigurations : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.MedicineName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pi => pi.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Frequency)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.Instructions)
                .HasMaxLength(1000);

            builder.Property(pi => pi.DurationInDays)
                .IsRequired();

            builder.HasOne(pi => pi.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pi => pi.PrescriptionId);
        }
    }
}
