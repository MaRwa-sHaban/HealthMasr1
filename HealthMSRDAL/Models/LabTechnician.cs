namespace HealthMSR.DAL.Models
{
    public class LabTechnician
    {
        public int Id { get; set; }
        public string NationalId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Speciality { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        public ICollection<Lab_Report> Lab_Reports { get; set; } = new List<Lab_Report>();
    }
}