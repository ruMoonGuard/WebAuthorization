using Microsoft.AspNetCore.Identity;

namespace Authorization.Models
{
    public class User : IdentityUser
    {
        public string Login { get; set; }
    }
}