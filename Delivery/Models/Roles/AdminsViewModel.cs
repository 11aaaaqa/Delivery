using System.Collections.Generic;
using Delivery.Models.Reg;

namespace Delivery.Models.Roles
{
    public class AdminsViewModel
    {
        public IList<User> MainAdmins { get; set; }
        public IList<User> Admins { get; set; }
        public IList<User> Moders { get; set; }
    }
}
