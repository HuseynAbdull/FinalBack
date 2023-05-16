using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.ViewModels.RegisterVM
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
