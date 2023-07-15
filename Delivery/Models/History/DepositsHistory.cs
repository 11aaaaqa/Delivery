using System;

namespace Delivery.Models.History
{
    public class DepositsHistory
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public uint Amount { get; set; }

        public DateTime Date { get; set; }
    }
}
