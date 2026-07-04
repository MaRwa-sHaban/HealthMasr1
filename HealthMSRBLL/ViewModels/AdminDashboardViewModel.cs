using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class AdminDashboardViewModel
    {
        public string ActiveSection { get; set; } = "overview";

        // Stats
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPharmacists { get; set; }
        public int TotalLabTechs { get; set; }

        public int TotalRadiologists { get; set; }
        // Lists
        public List<Doctor> Doctors { get; set; } = new();
        public List<Pharmacist> Pharmacists { get; set; } = new();
        public List<LabTechnician> LabTechnicians { get; set; } = new();
        public List<Patient> Patients { get; set; } = new();
        public List<Organization> Organizations { get; set; } = new();
        public List<Radiologist> Radiologists { get; set; } = new();

        // Filter
        public string FilterSpecialty { get; set; }
        public string SearchQuery { get; set; }
    }
}