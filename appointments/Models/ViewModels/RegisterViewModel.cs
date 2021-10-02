using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace appointments.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        [StringLength(100, ErrorMessage ="Hasło {0} musi mieć conajmniej {2} znaków", MinimumLength =6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name= "Potwierdź hasło")]
        [Compare("Password", ErrorMessage ="Hasła się nie zgadzają")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name ="Rola")]
        public string RoleName { get; set; }
    }
}
