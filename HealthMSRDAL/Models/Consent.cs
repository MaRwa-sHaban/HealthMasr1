namespace HealthMSR.DAL.Models
{
    public class Consent
    {
        public int ConsentId { get; set; }
        public string Scope { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}