namespace HealthMSR.DAL.Models
{
    public class Radiologist
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = "General Radiology";
        public string PasswordHash { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Radiology_Report> Radiology_Reports { get; set; } = new List<Radiology_Report>();
    }
}