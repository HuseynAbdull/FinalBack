using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public  string? Name { get; set; }

        [StringLength(100)]
        public string? Surname { get; set; }

    }
}
