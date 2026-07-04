using System.ComponentModel.DataAnnotations;

namespace HealthMSR.DAL.Models
{
    public class Prescription_Item
    {
        public int ItemId { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }

        [Required]
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }

        public Prescription Prescription { get; set; }
        public Medication Medication { get; set; }
    }
}