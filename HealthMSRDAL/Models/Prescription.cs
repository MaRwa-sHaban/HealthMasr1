namespace HealthMSR.DAL.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int EncounterId { get; set; }
        public DateTime DatePrescribed { get; set; }
        public string Notes { get; set; }

        public Encounter Encounter { get; set; }
        public ICollection<Prescription_Item> Items { get; set; } = new List<Prescription_Item>();
        public ICollection<Dispense> Dispenses { get; set; } = new List<Dispense>();
    }
}