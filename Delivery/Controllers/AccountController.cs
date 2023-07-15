using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Delivery.Models;
using Delivery.Models.History;
using Delivery.Models.Reg;
using Delivery.Models.Replenish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ApplicationContext context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                User user = new User{UserName = model.UserName, City = string.Empty, Address = string.Empty};
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Products", "Product");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new Login{ReturnUrl = returnUrl});
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Products", "Product");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty,"Неправильный логин или пароль");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Products", "Product");
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult Replenish(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Replenish(ReplenishViewModel model, string returnUrl)
        {
            if (model.Amount == 0)
            {
                ModelState.AddModelError(string.Empty, "Значение поля 'Сумма' должно быть больше 0");
            }
            if (ModelState.IsValid)
            {
                var currentUserName = User.Identity.Name;

                var currentUser = await userManager.Users.FirstOrDefaultAsync(options => options.UserName == currentUserName);

                currentUser.Money += model.Amount;

                var result = await userManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    await context.Deposits.AddAsync(new DepositsHistory
                        {UserName = currentUserName, Amount = model.Amount, Date = DateTime.Now});
                    await context.SaveChangesAsync();

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
        public IActionResult Orders()
        {
            var orders = context.Orders.Where(x => x.UserName == User.Identity.Name).ToList();

            var ordersSorted =
                from m in orders
                orderby m.Date descending 
                select m;

            return View(ordersSorted);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Deposits()
        {
            var deposits = context.Deposits.Where(x => x.UserName == User.Identity.Name).ToList();

            var depositsSorted =
                from m in deposits
                orderby m.Date descending
                select m;

            return View(depositsSorted);
        }
    }
}
