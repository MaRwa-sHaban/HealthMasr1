using System.ComponentModel.DataAnnotations;

namespace HealthMSR.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string NationalId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}