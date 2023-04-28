using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.Models
{
    public class Settings : BaseEntity
    {
        [StringLength(255)]
        public string Key { get; set; }

        [StringLength(255)]
        public string Value { get; set; }
    }
}
