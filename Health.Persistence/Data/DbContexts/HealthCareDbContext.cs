using Health.Domain.Entities.AppointmentModule;
using Health.Domain.Entities.ConsultationModule;
using Health.Domain.Entities.DoctorModule;
using Health.Domain.Entities.PatientModule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Health.Persistence.Data.DbContexts
{
    public class HealthCareDbContext : DbContext
    {

        public HealthCareDbContext(DbContextOptions<HealthCareDbContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorGeneratedSlots> DoctorGeneratedSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
    }
}
