using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class DispenseConfig : IEntityTypeConfiguration<Dispense>
    {
        public void Configure(EntityTypeBuilder<Dispense> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.Prescription)
                .WithMany(p => p.Dispenses)
                .HasForeignKey(d => d.PrescriptionId);

            builder.HasOne(d => d.Pharmacist)
                .WithMany(p => p.Dispenses)
                .HasForeignKey(d => d.PharmacistId);
        }
    }
}