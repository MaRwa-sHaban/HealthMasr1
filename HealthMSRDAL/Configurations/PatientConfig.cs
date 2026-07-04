using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class PatientConfig : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.PatientId);

            builder.HasIndex(p => p.NationalId).IsUnique();
            builder.Property(p => p.NationalId).IsRequired().HasMaxLength(14);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(50);

            builder.HasOne(p => p.MedicalRecord)
                .WithOne(m => m.Patient)
                .HasForeignKey<Medical_Record>(m => m.PatientId);

            builder.HasMany(p => p.Consents)
                .WithOne(c => c.Patient)
                .HasForeignKey(c => c.PatientId);

            builder.HasMany(p => p.Conditions)
                .WithOne(c => c.Patient)
                .HasForeignKey(c => c.PatientId);

            builder.HasMany(p => p.Encounters)
                .WithOne(e => e.Patient)
                .HasForeignKey(e => e.PatientId);
        }
    }
}