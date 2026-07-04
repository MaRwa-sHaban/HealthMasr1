using HealthMSR.DAL.Enums;

namespace HealthMSR.DAL.Models
{
    public class Dispense
    {
        public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int PharmacistId { get; set; }
        public DateTime DispenseDate { get; set; }
        public DispenseStatus Status { get; set; }

        public Prescription Prescription { get; set; }
        public Pharmacist Pharmacist { get; set; }
    }
}