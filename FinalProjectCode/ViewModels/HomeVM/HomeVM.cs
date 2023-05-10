using FinalProjectCode.Models;

namespace FinalProjectCode.ViewModels.HomeVM
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<BrandLogo> BrandLogos { get; set; }

        public IEnumerable<Gender>? Genders { get; set; }

    }
}
