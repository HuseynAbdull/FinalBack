using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;

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

        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductType> productTypes = await _context.ProductTypes

                .Where(p =>p.IsDeleted == false).ToListAsync();
            return View(productTypes);
        }

    }
}
