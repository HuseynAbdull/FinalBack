using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace FinalProjectCode.Models
{
    public class ProductImage : BaseEntity
    {
        [StringLength(255)]
        public string Image { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }


    }
}
