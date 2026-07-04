using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class RadiologyDashboardViewModel
    {
        public Radiologist Radiologist { get; set; }
        public string ActiveSection { get; set; } = "overview";
        public List<PendingRadItem> PendingScans { get; set; } = new();
        public int CompletedToday { get; set; }
        public Patient SearchedPatient { get; set; }
        public string SearchError { get; set; }
        public List<Radiology_Report> PatientRadReports { get; set; } = new();
    }

    public class PendingRadItem
    {
        public Radiology_Report RadReport { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
    }
}