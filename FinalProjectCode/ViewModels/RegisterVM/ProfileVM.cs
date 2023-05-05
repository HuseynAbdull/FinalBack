using FinalProjectCode.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.ViewModels.RegisterVM
{
    public class ProfileVM
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public IEnumerable<Address>? Addresses { get; set; }

        public IEnumerable<Order> Orders { get; set; }
    }
}
