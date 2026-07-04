using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class EncounterConfig : IEntityTypeConfiguration<Encounter>
    {
        public void Configure(EntityTypeBuilder<Encounter> builder)
        {
            builder.HasKey(e => e.EncounterId);

            builder.HasOne(e => e.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(e => e.PatientId);

            builder.HasOne(e => e.Doctor)
                .WithMany(d => d.Encounters)
                .HasForeignKey(e => e.PractitionerId);

            builder.HasMany(e => e.LabReports)
                .WithOne(l => l.Encounter)
                .HasForeignKey(l => l.EncounterId);

            builder.HasMany(e => e.Prescriptions)
                .WithOne(p => p.Encounter)
                .HasForeignKey(p => p.EncounterId);

            builder.HasMany(e => e.RadiologyReports)
                .WithOne(r => r.Encounter)
                .HasForeignKey(r => r.EncounterId);
        }
    }
}