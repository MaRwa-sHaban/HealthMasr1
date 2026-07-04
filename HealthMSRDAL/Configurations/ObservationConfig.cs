using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class ObservationConfig : IEntityTypeConfiguration<Observation>
    {
        public void Configure(EntityTypeBuilder<Observation> builder)
        {
            builder.HasKey(o => o.ObservationId);

            builder.HasOne(o => o.Doctor)
                .WithMany(d => d.Observations)
                .HasForeignKey(o => o.PractitionerId);

            builder.HasOne(o => o.ExternalSystem)
                .WithMany(s => s.Observations)
                .HasForeignKey(o => o.SystemId);
        }
    }
}