namespace HealthMSR.DAL.Models
{
    public class Observation
    {
        public int ObservationId { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public DateTime DateTime { get; set; }

        public int PractitionerId { get; set; }
        public Doctor Doctor { get; set; }

        public int SystemId { get; set; }
        public External_System ExternalSystem { get; set; }
    }
}