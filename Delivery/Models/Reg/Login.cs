using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Reg
{
    public class Login
    {
        [Required]
        [Display(Name="Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name="Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
