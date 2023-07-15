using System;

namespace Delivery.Helpers
{
    public class DeliveryTime
    {
        public static int Time()
        {
            Random random = new Random();
            int rnd;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday)
            {
                rnd = random.Next(1, 4);
                switch (rnd)
                {
                    case 1: return 30;
                    case 2: return 35;
                    case 3: return 40;
                    case 4: return 45;
                }
            }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
            {
                rnd = random.Next(1, 5);
                switch (rnd)
                {
                    case 1: return 20;
                    case 2: return 25;
                    case 3: return 30;
                    case 4: return 35;
                    case 5: return 40;
                }
            }
            
            rnd = random.Next(1, 4);

            switch (rnd)
            {
                case 1: return 15;
                case 2: return 20;
                case 3: return 25;
            }

            return 30;
        }
    }
}
