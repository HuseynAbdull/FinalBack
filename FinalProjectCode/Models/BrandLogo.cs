using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class BrandLogo : BaseEntity
    {
        [StringLength(255)]
        public string? Image { get; set; }

    }
}
