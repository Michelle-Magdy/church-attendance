
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ChurchAttendance.Core.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Person name is required")]
        [RegularExpression(@"^01[0125]\d{8}$", ErrorMessage = "Enter a valid Egyptian phone number (e.g., 01012345678)")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password",ErrorMessage ="Password and confirm password didn't match")]
        public string? ConfirmPassword {  get; set; } = string.Empty;

        [Required(ErrorMessage ="Person name is required")]
        [MaxLength(200,ErrorMessage ="Person Name cannot be greater than 200 chars")]
        public string? PersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confessor is required")]
        [MaxLength(200, ErrorMessage = "Confessor cannot be greater than 200 chars")]
        public string? Confessor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot be greater than 200 chars")]
        public string? Address { get; set; } = string.Empty;

       // for saas app
        public string? ChurchCode { get; set; } = string.Empty;
    }
}
