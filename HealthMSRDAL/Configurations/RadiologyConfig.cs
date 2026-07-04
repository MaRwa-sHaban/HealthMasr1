using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class RadiologyConfig : IEntityTypeConfiguration<Radiology_Report>
    {
        public void Configure(EntityTypeBuilder<Radiology_Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Encounter)
                .WithMany(e => e.RadiologyReports)
                .HasForeignKey(r => r.EncounterId);

            builder.HasOne(r => r.Radiologist)
                .WithMany(r => r.Radiology_Reports)
                .HasForeignKey(r => r.RadiologistId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}