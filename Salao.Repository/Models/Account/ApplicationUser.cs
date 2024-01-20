using Microsoft.AspNetCore.Identity;

namespace Salao.Repository.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        public string? GenericPassword { get; set; }
        public string FullName { get; set; }
    }
}
