using System.ComponentModel.DataAnnotations;

namespace KhielsSkincare.Models.ViewModels
{
	public class LoginViewModel
	{
        
        [Required(ErrorMessage = "Không được để trống")]
        public string UserName { get; set; }
        
        [DataType(DataType.Password), Required(ErrorMessage = "Không được để trống")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
