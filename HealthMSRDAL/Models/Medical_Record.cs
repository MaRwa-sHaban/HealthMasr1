using System.ComponentModel.DataAnnotations;

namespace HealthMSR.DAL.Models
{
    public class Medical_Record
    {
        [Key]
        public int RecordId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateCreated { get; set; }
        public string BloodType { get; set; }
        public string GlobalStatus { get; set; }

        public Patient Patient { get; set; }
    }
}