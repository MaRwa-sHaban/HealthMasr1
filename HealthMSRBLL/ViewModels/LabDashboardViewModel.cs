using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class LabDashboardViewModel
    {
        public LabTechnician LabTechnician { get; set; }
        public string ActiveSection { get; set; } = "overview";
        public List<PendingLabItem> PendingLabs { get; set; } = new();
        public int CompletedToday { get; set; }
        public Patient SearchedPatient { get; set; }
        public string SearchError { get; set; }
        public List<Lab_Report> PatientLabReports { get; set; } = new();
    }

    public class PendingLabItem
    {
        public Lab_Report LabReport { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
    }
}