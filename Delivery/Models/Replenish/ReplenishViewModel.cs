using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Replenish
{
    public class ReplenishViewModel
    {
        [Required]
        [Display(Name = "Сумма")]
        public uint Amount { get; set; }
    }
}
