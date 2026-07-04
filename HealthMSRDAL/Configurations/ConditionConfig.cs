using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class ConditionConfig : IEntityTypeConfiguration<Condition>
    {
        public void Configure(EntityTypeBuilder<Condition> builder)
        {
            builder.HasKey(c => c.ConditionId);

            builder.HasOne(c => c.Patient)
                .WithMany(p => p.Conditions)
                .HasForeignKey(c => c.PatientId);
        }
    }
}