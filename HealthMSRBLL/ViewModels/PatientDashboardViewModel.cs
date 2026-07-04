using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; }
        public Medical_Record MedicalRecord { get; set; }
        public List<Encounter> Encounters { get; set; } = new();
        public List<Prescription> Prescriptions { get; set; } = new();
        public List<Lab_Report> LabReports { get; set; } = new();
        public List<Radiology_Report> RadiologyReports { get; set; } = new();
        public List<Observation> Observations { get; set; } = new();
        public List<Condition> Conditions { get; set; } = new();
        public string ActiveSection { get; set; } = "overview";
    }
}