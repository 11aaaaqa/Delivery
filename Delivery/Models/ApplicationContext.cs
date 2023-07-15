using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delivery.Models.History;
using Delivery.Models.Reg;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Delivery.Models.Product;
using Newtonsoft.Json;

namespace Delivery.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Products> Products { get; set; }
        public DbSet<DepositsHistory> Deposits { get; set; }
        public DbSet<OrdersHistory> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrdersHistory>()
                .Property(p => p.ProductsNames)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v));

            builder.Entity<Products>().HasData(new Products
            {
                Id = new Guid("22646be9-9f9d-46e4-b73c-a0c221343c72"),
                ProductName = "Вода",
                ExpirationDate = "1 год",
                Manufacturer = "Волжанка",
                Price = 90
            });
        }
    }
}
