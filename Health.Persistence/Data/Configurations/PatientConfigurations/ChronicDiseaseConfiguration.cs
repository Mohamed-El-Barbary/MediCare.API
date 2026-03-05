using Health.Domain.Entities.PatientModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.PatientConfigurations
{
    internal class ChronicDiseaseConfiguration : IEntityTypeConfiguration<ChronicDisease>
    {
        public void Configure(EntityTypeBuilder<ChronicDisease> builder)
        {
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Relationship with PatientChronicDisease
            builder.HasMany(c => c.PatientChronicDiseases)
                   .WithOne(pc => pc.ChronicDisease)
                   .HasForeignKey(pc => pc.ChronicDiseaseId);
        }
    }
}
