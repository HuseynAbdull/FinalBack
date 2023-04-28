using FinalProjectCode.Models;

namespace FinalProjectCode.ViewModels.ShopVM
{
    public class ShopVM
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<ProductType> ProductTypes { get; set; }

        public IEnumerable<Gender> Genders { get; set; }

    }
}
