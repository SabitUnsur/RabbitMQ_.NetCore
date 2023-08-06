﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RabbitMq.Web.ExcelReport.Services;

namespace RabbitMq.Web.ExcelReport.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<IActionResult> Login()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email,string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);
            if (hasUser == null)
            {
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser,Password,true,false);
            
            if (!signInResult.Succeeded)
            {
                return View();
            }

            return RedirectToAction(nameof(HomeController.Index),"Home");
        }
    }
}
