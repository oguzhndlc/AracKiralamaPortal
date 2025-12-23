using Microsoft.AspNetCore.Identity;

namespace AracKiralamaPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ProfileURL { get; set; }
    }
}
