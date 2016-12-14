using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hans.AspNetCore.Identity.Marten.Data.Domains;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hans.AspNetCore.Identity.Marten.Models.UserAdminViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hans.AspNetCore.Identity.Marten.Controllers
{
    public class UserAdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        //
        // GET: /Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await userManager.FindByIdAsync(id);

            ViewData["RoleNames"] = await userManager.GetRolesAsync(user);

            return View(user);
        }

        //
        // GET: /Users/Create
        public IActionResult Create()
        {
            //Get the list of Roles
            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<IActionResult> Create(RegisterUserViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = userViewModel.Email, Email = userViewModel.Email };
                var adminresult = await userManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await userManager.AddToRolesAsync(user, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().ToString());
                            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First().ToString());
                    ViewData["RoleId"] = new SelectList(roleManager.Roles, "Name", "Name");
                    return View();

                }
                return RedirectToAction("Index");
            }

            ViewData["RoleId"] = new SelectList(roleManager.Roles, "Name", "Name");

            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            var userRoles = await userManager.GetRolesAsync(user);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                RoleList = roleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return new NotFoundResult();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await userManager.GetRolesAsync(user);

                selectedRole = selectedRole ?? new string[] { };

                var result = await userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Something failed.");

            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new BadRequestResult();
                }

                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return new NotFoundResult();
                }
                var result = await userManager.DeleteAsync(user);
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
