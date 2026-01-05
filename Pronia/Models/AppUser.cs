using Microsoft.AspNetCore.Identity;

namespace Pronia.Models
{
    public class AppUser: IdentityUser
    {
        public string Fullname { get; set; }    
    }
}
