using Microsoft.AspNetCore.Identity;

namespace KhielsSkincare.Models
{
    public class AppUser : IdentityUser
    {
        public string Address { get; set; }
    }

}
