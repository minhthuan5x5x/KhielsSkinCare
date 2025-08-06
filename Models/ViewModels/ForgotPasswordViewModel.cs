using System.ComponentModel.DataAnnotations;

namespace KhielsSkincare.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
