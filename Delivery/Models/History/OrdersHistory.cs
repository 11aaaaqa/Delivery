using System;
using System.Collections.Generic;

namespace Delivery.Models.History
{
    public class OrdersHistory
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public uint Amount { get; set; }

        public string UserName { get; set; }

        public string Address { get; set; }

        public List<string> ProductsNames { get; set; }
    }
}
