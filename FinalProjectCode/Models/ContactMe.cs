using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class ContactMe :BaseEntity
    {

        public AppUser? User { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
    }
}
