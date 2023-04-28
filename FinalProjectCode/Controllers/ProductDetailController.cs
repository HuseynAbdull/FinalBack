using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly AppDbContext _context;
        public ProductDetailController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? productid)
        {
            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false).Include(p=>p.ProductImages).ToListAsync();

            Product product = products.FirstOrDefault(p => p.Id == productid);

            return View(product);
        }
    }
}
