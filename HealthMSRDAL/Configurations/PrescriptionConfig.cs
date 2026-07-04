using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class PrescriptionConfig : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p => p.PrescriptionId);

            builder.HasOne(p => p.Encounter)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(p => p.EncounterId);

            builder.HasMany(p => p.Items)
                .WithOne(i => i.Prescription)
                .HasForeignKey(i => i.PrescriptionId);

            builder.HasMany(p => p.Dispenses)
                .WithOne(d => d.Prescription)
                .HasForeignKey(d => d.PrescriptionId);
        }
    }
}