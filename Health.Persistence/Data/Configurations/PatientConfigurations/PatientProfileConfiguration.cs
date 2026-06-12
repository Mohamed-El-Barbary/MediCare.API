using Health.Domain.Entities.PatientModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.PatientConfigurations
{
    internal class PatientProfileConfiguration : IEntityTypeConfiguration<PatientProfile>
    {
        public void Configure(EntityTypeBuilder<PatientProfile> builder)
        {
            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.DisplayName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Gender)
                   .IsRequired();

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            builder.Property(p => p.JoinDate)
                .HasDefaultValueSql("GETDATE()");

            // Owned Entity: Address
            builder.OwnsOne(p => p.Address, a =>
            {
                a.Property(ad => ad.Street).HasMaxLength(200);
                a.Property(ad => ad.City).HasMaxLength(100);
                a.Property(ad => ad.Country).HasMaxLength(50);
            });

            // Relationship with PatientChronicDisease
            builder.HasMany(p => p.PatientChronicDiseases)
                   .WithOne(pc => pc.Patient)
                   .HasForeignKey(pc => pc.PatientId);

            // Owned Entity : EmergencyContact
            builder.OwnsOne(p => p.EmergencyContact);
        }
    }
}
