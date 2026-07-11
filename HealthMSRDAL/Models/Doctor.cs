using System.ComponentModel.DataAnnotations;

namespace HealthMSR.DAL.Models
{
    public class Doctor
    {
        public int PractitionerId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;
        public string Specialty { get; set; } = "General Practice";
        public string LicenseNumber { get; set; } = string.Empty;
        public int OrganizationId { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Organization Organization { get; set; }
        public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
        public ICollection<Observation> Observations { get; set; } = new List<Observation>();
        public string Email { get;  set; }
    }
}