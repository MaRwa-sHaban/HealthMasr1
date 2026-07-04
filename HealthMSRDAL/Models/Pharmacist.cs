namespace HealthMSR.DAL.Models
{
    public class Pharmacist
    {
        public int Id { get; set; }
        public string NationalId { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; } = string.Empty;

        public ICollection<Dispense> Dispenses { get; set; } = new List<Dispense>();
        public int LicenceNumber { get; internal set; }
    }
}