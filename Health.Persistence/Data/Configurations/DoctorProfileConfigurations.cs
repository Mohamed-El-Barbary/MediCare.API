using Health.Domain.Entities.DoctorModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Persistence.Data.Configurations
{
    public class DoctorProfileConfigurations : IEntityTypeConfiguration<DoctorProfile>
    {
        public void Configure(EntityTypeBuilder<DoctorProfile> builder)
        {
            builder.Property(d => d.UserId)
                   .IsRequired()
                   .HasMaxLength(450); 

            builder.Property(d => d.Specialization)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Bio)
                   .HasMaxLength(1000);

            builder.Property(d => d.ClinicLocation)
                   .HasMaxLength(200);

            builder.Property(d => d.SyndicateCardUrl)
                   .HasMaxLength(500);

            builder.Property(d => d.DoctorPictureUrl)
                   .HasMaxLength(500);

            builder.Property(d => d.Rating)
                   .HasColumnType("decimal(3,2)");

            builder.HasMany(p => p.DoctorSchedule)
                   .WithOne(d => d.DoctorProfile)
                   .HasForeignKey(d => d.DoctorProfileId);

            builder.OwnsOne(d => d.Address, a =>
            {
                a.Property(p => p.Street).HasMaxLength(200);
                a.Property(p => p.City).HasMaxLength(100);
                a.Property(p => p.Country).HasMaxLength(100);
            });
        }
    }
}
