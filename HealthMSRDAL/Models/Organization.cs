using System.ComponentModel.DataAnnotations;

namespace HealthMSR.DAL.Models
{
    public class Organization
    {
        public int OrganizationId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string OrgType { get; set; } = "Hospital";
        public string TaxId { get; set; } = "000000000";
        public string Address { get; set; } = "Cairo";

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}