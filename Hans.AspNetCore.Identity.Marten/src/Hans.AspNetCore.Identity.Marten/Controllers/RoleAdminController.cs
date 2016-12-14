using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Microsoft.AspNetCore.Identity;
using Hans.AspNetCore.Identity.Marten.Models.RoleAdminViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hans.AspNetCore.Identity.Marten.Controllers
{
    public class RoleAdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(roleManager.Roles);
        }

        //
        // GET: /Roles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }

            var role = await roleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<IdentityUser>();

            // Get the list of Users in this Role
            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewData["Users"] = users;
            ViewData["UserCount"] = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleresult = await roleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    ModelState.AddModelError("", roleresult.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new NotFoundResult();
            }
            var roleModel = new RoleViewModel { Id = role.Id, Name = role.Name };
            return View(roleModel);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await roleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return new NotFoundResult();
            }

            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new BadRequestResult();
                }
                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return new NotFoundResult();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await roleManager.DeleteAsync(role);
                }
                else
                {
                    result = await roleManager.DeleteAsync(role);
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
