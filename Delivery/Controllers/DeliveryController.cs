using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delivery.Models.Cart;
using Delivery.Models.Reg;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Delivery.Helpers;
using Delivery.Models;
using Delivery.Models.History;
using Microsoft.AspNetCore.Http;

namespace Delivery.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly ApplicationContext context;
        private readonly UserManager<User> userManager;

        public DeliveryController(UserManager<User> userManager, ApplicationContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Address(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Address(User model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var currentUserName = User.Identity.Name;

                var user = await userManager.Users.FirstOrDefaultAsync(options => options.UserName == currentUserName);

                user.City = model.City;
                user.Address = model.Address;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    CookieOptions options = new CookieOptions {Expires = DateTime.Now.AddYears(999)};
                    
                    Response.Cookies.Append("address", model.City + ", " + model.Address, options); //TODO

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Cart", "Delivery");
                }
            }
            return View(model);
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var currentUser = await context.Users.FirstAsync(x => x.UserName == User.Identity.Name);
            
            var cart = SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, "cart");
            
            var total = Calculate("cart");

            ViewBag.money = currentUser.Money;
            ViewBag.products = cart;
            ViewBag.totalPrice = total;

            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddToCart(Guid id)
        {
            if (SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, "cart") == null)
            {
                var cart = new List<CartViewModel>
                {
                    new CartViewModel {Product = context.Products.Find(id), Quantity = 1}
                };
                SessionExtension.Set(HttpContext.Session, "cart", cart);
            }
            else
            {
                var cart = SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, "cart");

                var prod = cart.FirstOrDefault(x => x.Product.Id == id);

                if (prod == null)
                {
                    cart.Add(new CartViewModel { Product = context.Products.Find(id), Quantity = 1 });
                }
                else
                {
                    prod.Quantity++;
                }
                SessionExtension.Set(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Products", "Product");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteFromCart(Guid id)
        {
            var cart = SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, "cart");

            foreach (var prod in cart)
            {
                if (prod.Product.Id == id)
                {
                    cart.Remove(prod);
                    SessionExtension.Set(HttpContext.Session, "cart", cart);
                    return RedirectToAction("Cart");
                }
            }
            return StatusCode(520);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Check()
        {
            var total = Calculate("cart");
            
            ViewBag.deliveryTime = DeliveryTime.Time();
            ViewBag.orderTime = DateTime.Now;
            ViewBag.address = Request.Cookies["address"];
            ViewBag.amount = total;

            SessionExtension.Set(HttpContext.Session,"cart",new List<CartViewModel>());

            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Order()
        {
            var currentUserName = User.Identity.Name;
            var user = await context.Users.FirstAsync(options => options.UserName == currentUserName);
            if (user.Address == string.Empty)
            {
                return RedirectToAction("Address");
            }
            
            var total = Calculate("cart");
            if (total != 0)
            {
                if (user.Money < total)
                {
                    return RedirectToAction("Replenish", "Account");
                }
                user.Money -= total;

                var cart = SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, "cart");

                var products = new List<string>();
                foreach (var prod in cart)
                {
                    products.Add(prod.Product.ProductName);
                }
                await context.Orders.AddAsync(new OrdersHistory
                {
                    Address = user.City + ", " + user.Address, Amount = total, Date = DateTime.Now, UserName = currentUserName, ProductsNames = products
                });

                await context.SaveChangesAsync();
                return RedirectToAction("Check");
            }

            return View();
        }

        public uint Calculate(string key)
        {
            var cart = SessionExtension.Get<List<CartViewModel>>(HttpContext.Session, key);
            uint total = 0;
            if (cart != null)
            {
                foreach (var prod in cart)
                {
                    total += prod.Product.Price * prod.Quantity;
                }
            }
            return total;
        }
    }
}
