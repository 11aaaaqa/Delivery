using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Roles
{
    public class CreateRole
    {
        
        [Required]
        [Display(Name = "Имя роли")]
        public string RoleName { get; set; }
    }
}
