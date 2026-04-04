using Health.Domain.Entities.PatientModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations.PatientConfigurations
{
    internal class PatientChronicDiseaseConfiguration : IEntityTypeConfiguration<PatientChronicDisease>
    {
        public void Configure(EntityTypeBuilder<PatientChronicDisease> builder)
        {
            // Composite Primary Key
            builder.HasKey(pc => new { pc.PatientId, pc.ChronicDiseaseId });

            // Properties
            builder.Property(pc => pc.DiagnosedAt)
                   .IsRequired();

            builder.Property(pc => pc.IsActive)
                   .HasDefaultValue(true);
        }
    }
}
