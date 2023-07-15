using System;
using System.Linq;
using Delivery.Models;
using Delivery.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Repository.Product
{
    public class ProductRepository
    {
        private readonly ApplicationContext context;

        public ProductRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public IQueryable<Products> GetProducts()
        {
            return context.Products.OrderBy(x => x.ProductName);
        }

        public void AddProduct(Products product)
        {
            context.Entry(product).State = EntityState.Added;
            context.SaveChanges();
        }

        public void ChangeProduct(Products product)
        {
            context.Entry(product).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteProduct(Guid id)
        {
            context.Products.Remove(new Products{Id = id});
            context.SaveChanges();
        }

        public Products GetProductById(Guid id)
        {
            return context.Products.FirstOrDefault(options => options.Id == id);
        }
    }
}
