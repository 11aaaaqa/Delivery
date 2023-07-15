using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Delivery.Models.Reg
{
    public class User : IdentityUser
    {
        [Required]
        [Display(Name = "Сумма")]
        public uint Money { get; set; }

        [Required]
        [Display(Name = "Город")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Адрес")]
        public string Address { get; set; }
    }
}
