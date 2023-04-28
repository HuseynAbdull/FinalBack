using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.ViewModels.BrandsVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Controllers
{
    public class BrandsController : Controller
    {
        private readonly AppDbContext _context;
        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            BrandsVM brandsVM = new BrandsVM
            {
                BrandLogos = await _context.BrandLogos.Where(b => b.IsDeleted == false).ToListAsync(),
            };

            return View(brandsVM);
        }
    }
}
