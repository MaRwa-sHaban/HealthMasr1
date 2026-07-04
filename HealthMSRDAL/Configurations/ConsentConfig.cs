using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class ConsentConfig : IEntityTypeConfiguration<Consent>
    {
        public void Configure(EntityTypeBuilder<Consent> builder)
        {
            builder.HasKey(c => c.ConsentId);

            builder.HasOne(c => c.Patient)
                .WithMany(p => p.Consents)
                .HasForeignKey(c => c.PatientId);
        }
    }
}