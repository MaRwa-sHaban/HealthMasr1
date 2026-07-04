using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class DoctorConfig : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.PractitionerId);
            builder.Property(d => d.FullName).IsRequired().HasMaxLength(100);

            builder.HasOne(d => d.Organization)
                .WithMany(o => o.Doctors)
                .HasForeignKey(d => d.OrganizationId);

            builder.HasMany(d => d.Encounters)
                .WithOne(e => e.Doctor)
                .HasForeignKey(e => e.PractitionerId);

            builder.HasMany(d => d.Observations)
                .WithOne(o => o.Doctor)
                .HasForeignKey(o => o.PractitionerId);
        }
    }
}