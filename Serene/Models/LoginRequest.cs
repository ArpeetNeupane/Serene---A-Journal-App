using System.ComponentModel.DataAnnotations;

namespace Serene.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "PIN is required")]
        [RegularExpression(@"^\d{4,6}$", ErrorMessage = "PIN must be 4 to 6 digits")]
        public string Pin { get; set; } = string.Empty;
    }
}
