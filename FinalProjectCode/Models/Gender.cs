using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectCode.Models
{
    public class Gender : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }

       public IEnumerable<Product>? Products { get; set;}

        [NotMapped]
        public IFormFile? File { get; set; }

    }
}
