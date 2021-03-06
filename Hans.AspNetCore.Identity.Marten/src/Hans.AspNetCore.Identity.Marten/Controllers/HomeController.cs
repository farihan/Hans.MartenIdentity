﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Hans.AspNetCore.Identity.Marten.Data.Domains;

namespace Hans.AspNetCore.Identity.Marten.Controllers
{
    public class HomeController : Controller
    {
        //private readonly RoleManager<IdentityRole> roleManager;
        //private readonly UserManager<IdentityUser> userManager;
        //private readonly SignInManager<IdentityUser> signInManager;

        //public HomeController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        //{
        //    this.roleManager = roleManager;
        //    this.userManager = userManager;
        //    this.signInManager = signInManager;
        //}

        public IActionResult Index()
        {
            //// arrange
            //var user1 = new IdentityUser("User1")
            //{
            //    Email = "user1@user.com",
            //    EmailConfirmed = true
            //};

            //var role1 = new IdentityRole("Role1");

            //// act
            //roleManager.CreateAsync(role1);
            //userManager.AddToRoleAsync(user1, "Role1");
            //userManager.CreateAsync(user1, "password");

            //var result = signInManager.SignInAsync(user1, false);

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
