using HealthMSR.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthMSR.DataAccess.Configurations
{
    public class LabReportConfig : IEntityTypeConfiguration<Lab_Report>
    {
        public void Configure(EntityTypeBuilder<Lab_Report> builder)
        {
            builder.HasKey(l => l.ReportId);

            builder.HasOne(l => l.Encounter)
                .WithMany(e => e.LabReports)
                .HasForeignKey(l => l.EncounterId);

            builder.HasOne(l => l.Patient)
                .WithMany()
                .HasForeignKey(l => l.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(l => l.LabTechnician)
                .WithMany(t => t.Lab_Reports)
                .HasForeignKey(l => l.LabTechnicianId);
        }
    }
}