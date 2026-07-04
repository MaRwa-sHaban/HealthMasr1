using HealthMSR.DAL.Enums;

namespace HealthMSR.DAL.Models
{
    public class Lab_Report
    {
        public int ReportId { get; set; }
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        public int LabTechnicianId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public string Results { get; set; } = "—";
        public LabStatus Status { get; set; } = LabStatus.Pending;

        public Encounter Encounter { get; set; }
        public Patient Patient { get; set; }
        public LabTechnician LabTechnician { get; set; }
    }
}