using FinalProjectCode.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.ViewModels.HomeVM
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<BrandLogo> BrandLogos { get; set; }

        public IEnumerable<Gender>? Genders { get; set; }
        
        public ContactMe? ContactMes { get; set; }


    }
}
