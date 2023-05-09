using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CatagoryController : Controller
    {
        private readonly AppDbContext _context;

        public CatagoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           
            return View(await _context.ProductTypes
                .Where(c=>c.IsDeleted == false).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Catagories = await _context.ProductTypes.Where(c => c.IsDeleted == false).ToListAsync();
            return View();
        }

    }
}
