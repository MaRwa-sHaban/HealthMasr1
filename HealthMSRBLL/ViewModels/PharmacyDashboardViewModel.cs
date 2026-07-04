using HealthMSR.DAL.Models;

namespace HealthMSR.ViewModels
{
    public class PharmacyDashboardViewModel
    {
        public Pharmacist Pharmacist { get; set; }
        public string ActiveSection { get; set; } = "overview";
        public int TotalPendingAll { get; set; }
        public int DispensedByMe { get; set; }
        public int DispensedTodayAll { get; set; }
        public int DispensedTodayByMe { get; set; }
        public List<PendingPrescriptionItem> PendingPrescriptions { get; set; } = new();
        public List<DispensedPrescriptionItem> DispensedPrescriptions { get; set; } = new();
        public Patient SearchedPatient { get; set; }
        public string SearchError { get; set; }
        public List<Prescription> PatientPrescriptions { get; set; } = new();

        //public static implicit operator PharmacyDashboardViewModel(PharmacyDashboardViewModel v)
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class PendingPrescriptionItem
    {
        public Prescription Prescription { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
        public int PatientId { get; set; }
    }

    public class DispensedPrescriptionItem
    {
        public Prescription Prescription { get; set; }
        public string PatientName { get; set; }
        public string PatientNationalId { get; set; }
        public string DispensedByName { get; set; }
        public DateTime DispensedDate { get; set; }
    }
}