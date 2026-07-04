namespace HealthMSR.DAL.Models
{
    public class External_System
    {
        public int SystemId { get; set; }
        public string SystemName { get; set; }
        public string SystemType { get; set; }
        public string AuthMethod { get; set; }

        public ICollection<Observation> Observations { get; set; } = new List<Observation>();
    }
}