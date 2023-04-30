using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.ProductVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ProductDetailController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index(int? productid)
        {
                IEnumerable<Product> products = await _context.Products
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .Where(p => p.IsDeleted == false)
                .Include(p => p.ProductImages).ToListAsync();
                Product product = products.FirstOrDefault(p => p.Id == productid);

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId= productid}
            };
                return View(productReviewVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> AddReview(Review review)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index",review);
            Product product = await _context.Products
             .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
              .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
              .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.ProductId);

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid) return View("Index", productReviewVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if(product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r => r.UserId == appUser.Id)) 
            {
                ModelState.AddModelError("Name", "Siz artiq fikir bildirmisiz");
                return View("Index", productReviewVM);
            }

            review.CreatedBy = $"{appUser.Name}{appUser.Surname}";
            review.CreatedAt =DateTime.UtcNow.AddHours(4);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index),new {id = product.Id});
        }


    /*    public async Task<IActionResult> Review(int? id)
        {
            if (id == null) { return BadRequest(); }

            Product product = await _context.Products
             .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
             .Include(p => p.BrandName)
               .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
              .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();


            return View(product);

        }*/
    }
}
