using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Delivery.Models;
using Delivery.Models.Reg;
using Delivery.Models.Roles;
using Delivery.Repository.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<User> userManager;
        private readonly ApplicationContext context;
        private readonly ProductRepository productRepository;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ApplicationContext context, ProductRepository productRepository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
            this.productRepository = productRepository;
        }

        [Authorize(Roles = "Администратор, Гл Админ")]
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = roleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = "Гл Админ")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "Гл Админ")]
        public async Task<IActionResult> CreateRole(CreateRole model)
        {
            bool exist = await roleManager.RoleExistsAsync(model.RoleName);

            if(exist) ModelState.AddModelError(string.Empty,"Роль с таким именем уже существует");

            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };

                var result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Admins");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Администратор, Гл Админ")]
        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [Authorize(Roles = "Администратор, Гл Админ")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"Такого пользователя не существует");
                return View(model);
            }
                
            if (ModelState.IsValid)
            {
                var result = await userManager.AddToRoleAsync(user, model.RoleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Admins");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpGet]
        public async Task<IActionResult> Admins(AdminsViewModel model)
        {
            var mainAdmins = await userManager.GetUsersInRoleAsync("Гл Админ");
            var admins = await userManager.GetUsersInRoleAsync("Администратор");
            var moders = await userManager.GetUsersInRoleAsync("Модератор");

            if (mainAdmins != null)
            {
                model.MainAdmins = mainAdmins;
            }
            if (admins != null)
            {
                model.Admins = admins;
            }
            if (moders != null)
            {
                model.Moders = moders;
            }
            return View(model);
        }

        [Authorize(Roles = "Гл Админ")]
        [HttpPost]
        public async Task<IActionResult> RemoveRoles(string id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);

            var roles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, roles);

            await userManager.UpdateSecurityStampAsync(user);

            return RedirectToAction("Admins");
        }

        [Authorize(Roles = "Модератор, Администратор, Гл Админ")]
        [HttpGet]
        public IActionResult ProductsManage()
        {
            var model = productRepository.GetProducts();
            return View(model);
        }
    }
}
