namespace HealthMSR.DAL.Models
{
    public class Medication
    {
        public int MedicationId { get; set; }
        public string DrugName { get; set; }
        public string Route { get; set; }

        public ICollection<Prescription_Item> Items { get; set; } = new List<Prescription_Item>();
    }
}