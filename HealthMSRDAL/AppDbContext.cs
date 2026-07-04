using HealthMSR.DAL.Models;
using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthMSR.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medical_Record> Medical_Records { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Medication> Medications { get; set; }

        public DbSet<Lab_Report> Lab_Reports { get; set; }
        public DbSet<External_System> External_Systems { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Consent> Consents { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Prescription_Item> Prescription_Items { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Dispense> Dispenses { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<LabTechnician> LabTechnicians { get; set; }
        public DbSet<Radiology_Report> Radiology_Reports { get; set; }
        public DbSet<Radiologist> Radiologists { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}