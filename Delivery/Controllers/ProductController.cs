using Microsoft.AspNetCore.Mvc;
using System;
using Delivery.Models.Product;
using Delivery.Repository.Product;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.Controllers
{
    public class ProductController : Controller
    {
        private ProductRepository repository;

        public ProductController(ProductRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Products()
        {
            var model= repository.GetProducts();
            return View(model);
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpPost]
        public IActionResult Create(Products model)
        {
            if (ModelState.IsValid)
            {
                repository.AddProduct(model);
                return RedirectToAction("ProductsManage", "Administration");
            }

            return View(model);
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpGet]
        public IActionResult Change()
        {
            return View();
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpPost]
        public IActionResult Change(Products model)
        {
            if (ModelState.IsValid)
            {
                repository.ChangeProduct(model);
                return RedirectToAction("ProductsManage", "Administration");
            }

            return View(model);
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            repository.DeleteProduct(id);
            return RedirectToAction("ProductsManage", "Administration");
        }
    }
}
