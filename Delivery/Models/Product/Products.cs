using System;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Models.Product
{
    public class Products
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Название продукта")]
        public string ProductName { get; set; }

        [Required]
        [Display(Name = "Цена")]
        public uint Price { get; set; }

        [Required]
        [Display(Name = "Срок годности")]
        public string ExpirationDate { get; set; }

        [Required]
        [Display(Name = "Производитель")]
        public string Manufacturer { get; set; }
    }
}
