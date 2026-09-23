using Microsoft.AspNetCore.Identity;

namespace IdentityNet9.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeCompleto { get; set; }
    }
}