using HealthMSR.DAL.Models;

namespace HealthMSR.Controllers
{
    internal class PendingPrescriptionItem
    {
        public Prescription Prescription { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
        public int PatientId { get; set; }
    }
}