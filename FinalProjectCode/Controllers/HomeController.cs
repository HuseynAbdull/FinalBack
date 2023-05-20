using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Migrations;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.HomeVM;
using FinalProjectCode.ViewModels.RegisterVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Text;

namespace FinalProjectCode.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult>  Index()
        {
            Models.ContactMe contactMe = new Models.ContactMe();

            HomeVM homeVM = new HomeVM
            {
                Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync(),
                BrandLogos = await _context.BrandLogos.Where(b => b.IsDeleted == false).ToListAsync(),
                Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync(),
                ContactMes = contactMe

            };


            return View(homeVM);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(HomeVM homeVM)
        {

            Models.ContactMe contactMe1 = new Models.ContactMe();
            if(homeVM.ContactMes.Name != null) 
            {
                contactMe1.Name = homeVM.ContactMes.Name;
                if (homeVM.ContactMes.Email != null)
                {
                    contactMe1.Email = homeVM.ContactMes.Email;
                    await _context.ContactMes.AddAsync(contactMe1);
                    await _context.SaveChangesAsync();
                }
            }


            return RedirectToAction("Index");

        }


        public async Task<IActionResult> SetSession()
        {
            HttpContext.Session.SetString("P133","Session Data");

            return Content("Session Elave Olundu");
        }

        public async Task<IActionResult> GetSession()
        {
            var ses = HttpContext.Session.GetString("P133");
            return Content(ses);
        }


    }
}
