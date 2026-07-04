using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class ExternalSystemConfig : IEntityTypeConfiguration<External_System>
    {
        public void Configure(EntityTypeBuilder<External_System> builder)
        {
            builder.HasKey(s => s.SystemId);

            builder.HasMany(s => s.Observations)
                .WithOne(o => o.ExternalSystem)
                .HasForeignKey(o => o.SystemId);
        }
    }
}