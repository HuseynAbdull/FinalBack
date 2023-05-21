using System.ComponentModel.DataAnnotations;

namespace FinalProjectCode.ViewModels.RegisterVM
{
    public class RegisterVM
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


      /* SAYTIN OZUNDE OLMADIGI UCUN ISTIFADE ETMEDIM
       *
       * [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }*/
    }
}
