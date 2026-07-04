using HealthMSR.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace HealthMSR.DAL.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required, MaxLength(14)]
        public string NationalId { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        public Medical_Record MedicalRecord { get; set; }
        public ICollection<Consent> Consents { get; set; } = new List<Consent>();
        public ICollection<Condition> Conditions { get; set; } = new List<Condition>();
        public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
    }
}