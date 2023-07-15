using Delivery.Models.Product;

namespace Delivery.Models.Cart
{
    public class CartViewModel
    {
        public Products Product { get; set; }
        
        public uint Quantity { get; set; }
    }
}
