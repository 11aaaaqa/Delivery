using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Roles
{
    public class AssignRoleViewModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Название роли")]
        public string RoleName { get; set; }
    }
}
