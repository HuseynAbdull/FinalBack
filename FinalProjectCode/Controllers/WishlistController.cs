using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using FinalProjectCode.ViewModels.WishlistVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinalProjectCode.Controllers
{
    public class WishlistController : Controller
    {

        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string wishlist= HttpContext.Request.Cookies["wishlist"];
            List<WishlistVM> wishlistVMs = null;
            if (wishlist != null)
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);
                foreach(WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = _context.Products.FirstOrDefault(p => p.Id == wishlistVM.Id && p.IsDeleted == false);
                    if (product != null)
                    {
                        wishlistVM.Title = product.Title;
                        wishlistVM.Image = product.MainImage;
                        wishlistVM.Price = product.Price;
                    }

                }

            }
            else
            {
               wishlistVMs = new List<WishlistVM>();
            }

            return View(wishlistVMs);

        }

        public async Task<IActionResult> AddWishlist(int? id)
        {
            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();

            Product product = products.FirstOrDefault(p => p.Id == id);

            if (id == null) { return BadRequest(); }

            if (product == null)
            {
                return NotFound();
            }


            string wishlist = HttpContext.Request.Cookies["wishlist"];

            if (string.IsNullOrWhiteSpace(wishlist))
            {
                List<WishlistVM> wishlistVMs = new List<WishlistVM>();

                WishlistVM wishlistVM = new WishlistVM
                {
                    Id = product.Id,
                    Image = product.MainImage,
                    Title = product.Title,
                    Price = product.Price,
                    Count = 1,
                    DiscountedPrice = product.DiscountedPrice
                };

                wishlistVMs.Add(wishlistVM);

                string strProducts = JsonConvert.SerializeObject(wishlistVMs);

                HttpContext.Response.Cookies.Append("wishlist", strProducts);

            }
            else
            {
                List<WishlistVM> wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);

                if (wishlistVMs.Exists(b => b.Id == id))
                {
                    wishlistVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    WishlistVM wishlistVM = new WishlistVM
                    {
                        Id = product.Id,
                        Image = product.MainImage,
                        Title = product.Title,
                        Price = product.Price,
                        Count = 1,
                        DiscountedPrice = product.DiscountedPrice

                    };

                    wishlistVMs.Add(wishlistVM);

                }

                


                string strProducts = JsonConvert.SerializeObject(wishlistVMs);

                HttpContext.Response.Cookies.Append("wishlist", strProducts);

            }


            return Ok();
        }

        public async Task<IActionResult> DeleteWishlist(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string wishlist = HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;

            if (string.IsNullOrWhiteSpace(wishlist)) { return BadRequest(); }

            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);
                if (wishlistVMs.Exists(b => b.Id == id))
                {
                    WishlistVM wishlistVM = wishlistVMs.Find(b => b.Id == id);
                    wishlistVMs.Remove(wishlistVM);
                    wishlist = JsonConvert.SerializeObject(wishlistVMs);
                    HttpContext.Response.Cookies.Append("wishlist", wishlist);
                }
                else
                {
                    return NotFound();
                }
            }
            foreach (WishlistVM wishlistVM in wishlistVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    wishlistVM.DiscountedPrice = product.DiscountedPrice;
                    wishlistVM.Price = product.Price;
                    wishlistVM.Title = product.Title;
                    wishlistVM.Image = product.MainImage;
                }
            }

            return PartialView("_WishlistPartialView", wishlistVMs);
        }

    }
}
