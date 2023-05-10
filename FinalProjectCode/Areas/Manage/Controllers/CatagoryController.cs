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
            ProductType productType = new ProductType();
            return View(productType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductType productType)
        {

            if(string.IsNullOrEmpty(productType.Name))
            {
                ModelState.AddModelError("","Ad bos ola bilmez");
                return View(productType);
            }

            IEnumerable<ProductType> productTypes = await _context.ProductTypes.Where(p=>p.IsDeleted == false).ToListAsync();
            if(productTypes.Any(p=>p.Name.ToLower().ToString().Trim() == productType.Name.ToLower().ToString().Trim()))
            {
                ModelState.AddModelError("", "Product type artiq var.Eyni Ola bilmez");
                return View(productType);
            }

            productType.CreatedAt = DateTime.UtcNow.AddHours(4);
            productType.CreatedBy = "System";
            productType.Name = productType.Name.ToLower().ToString().Trim();
            

            await _context.ProductTypes.AddAsync(productType);
            await _context.SaveChangesAsync();
            

           return View();
        }

    }
}
