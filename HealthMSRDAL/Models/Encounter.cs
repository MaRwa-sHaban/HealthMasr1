namespace HealthMSR.DAL.Models
{
    public class Encounter
    {
        public int EncounterId { get; set; }
        public int PatientId { get; set; }
        public int PractitionerId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);
        public string Status { get; set; } = string.Empty;

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }

        public ICollection<Lab_Report> LabReports { get; set; } = new List<Lab_Report>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<Radiology_Report> RadiologyReports { get; set; } = new List<Radiology_Report>();
    }
}