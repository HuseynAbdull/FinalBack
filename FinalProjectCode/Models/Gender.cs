using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class Gender : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }

       public IEnumerable<Product>  Products { get; set;}

    }
}
