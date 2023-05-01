using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectCode.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }

        [StringLength(3000)]

        public string? Description { get; set; }

        [StringLength(255)]

        public string? UPC { get; set; }


        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }

        [StringLength(50)]
        public string? Model { get; set; }

        public Gender Gender { get; set; }
        public int GenderId { get; set; }

        [StringLength(255)]

        public string? BrandName { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }

        [Column(TypeName = "money")]

        public double? DiscountedPrice { get; set; }


        public int Count { get; set; }
        [StringLength(255)]
        public string MainImage { get; set; }

        public bool IsNewArrival { get; set; }

        public IEnumerable<Basket>? Baskets { get; set; }

        public IEnumerable<ProductImage> ProductImages { get; set;}

        public IEnumerable<Review>? Reviews { get; set; }


    }
}
