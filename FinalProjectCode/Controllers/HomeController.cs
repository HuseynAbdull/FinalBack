using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.ViewModels.HomeVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            HomeVM homeVM = new HomeVM
            {
                Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync(),
                BrandLogos =await _context.BrandLogos.Where(b => b.IsDeleted == false).ToListAsync(),
                Genders = await _context.Genders.Where(g=>g.IsDeleted == false).ToListAsync(),
            };

            return View(homeVM);
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
