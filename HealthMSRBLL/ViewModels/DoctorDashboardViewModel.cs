using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public Doctor Doctor { get; set; }
        public string ActiveSection { get; set; } = "overview";
        public Patient SearchedPatient { get; set; }
        public string SearchError { get; set; }

        // Stats
        public int TodayPatients { get; set; }
        public int TodayPrescriptions { get; set; }
        public int TodayLabRequests { get; set; }
        public int TodayRadRequests { get; set; }

        // Patient Data
        public List<Encounter> PatientEncounters { get; set; } = new();
        public List<Prescription> PatientPrescriptions { get; set; } = new();
        public List<Lab_Report> PatientLabReports { get; set; } = new();
        public List<Radiology_Report> PatientRadReports { get; set; } = new();
        public List<Observation> PatientObservations { get; set; } = new();
        public List<Medication> Medications { get; set; } = new();
        public object MyLabRequests { get; set; }
        public object MyRadRequests { get; set; }
        public int Age { get; set; }
    }
}