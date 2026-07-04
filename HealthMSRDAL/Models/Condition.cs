namespace HealthMSR.DAL.Models
{
    public class Condition
    {
        public int ConditionId { get; set; }
        public string ClinicalStatus { get; set; }
        public DateTime OnsetDate { get; set; }
        public string CodeTerm { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}