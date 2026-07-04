using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class PrescriptionItemConfig : IEntityTypeConfiguration<Prescription_Item>
    {
        public void Configure(EntityTypeBuilder<Prescription_Item> builder)
        {
            builder.HasKey(i => i.ItemId);
            builder.Property(i => i.Dosage).IsRequired();

            builder.HasOne(i => i.Prescription)
                .WithMany(p => p.Items)
                .HasForeignKey(i => i.PrescriptionId);

            builder.HasOne(i => i.Medication)
                .WithMany(m => m.Items)
                .HasForeignKey(i => i.MedicationId);
        }
    }
}