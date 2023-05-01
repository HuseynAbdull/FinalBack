using FinalProjectCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Controllers
{

    /*[Authorize(Roles ="Member")]*/
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser>_userManager;

        public OrderController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
           /* AppUser appUser = await _userManager.Users.Include(u=>u.Address)*/


            return View();
        }
    }
}
