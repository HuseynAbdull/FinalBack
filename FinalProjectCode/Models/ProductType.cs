using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class ProductType : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }

        IEnumerable<Product> Products { get; set;}

    }
}
