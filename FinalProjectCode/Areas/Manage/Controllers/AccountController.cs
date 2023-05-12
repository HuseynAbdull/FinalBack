using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.RegisterVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("Manage")]
    
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) { return View(loginVM); }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);
            if (appUser == null  || await _userManager.IsInRoleAsync(appUser, "SuperAdmin") == false)
            {
                if(appUser == null || await _userManager.IsInRoleAsync(appUser, "Admin") == false)
                {
                    ModelState.AddModelError("", "Email ve ya Password Yalnisdir");
                    return View(loginVM);
                }
                
            }

 
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, false, true);
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email ve ya Password Yalnisdir");
                return View(loginVM);
            }
            return RedirectToAction("index", "dashboard", new { areas = "manage" });








        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login");
        }


    }
}
