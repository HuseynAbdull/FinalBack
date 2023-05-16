using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProjectCode.ViewModels.ShopVM
{
    public class ShopVM
    {
        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<ProductType> ProductTypes { get; set; }

        public IEnumerable<Gender> Genders { get; set; }

        public string? Range { get; set; }

    }
}
