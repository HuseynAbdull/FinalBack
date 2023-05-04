using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Drawing;

namespace FinalProjectCode.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = null;
            if (basket != null)
            {
                basketVMs=JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = _context.Products.FirstOrDefault(p => p.Id == basketVM.Id && p.IsDeleted == false);
                    if (product != null)
                    {
                        basketVM.Title = product.Title;
                        basketVM.Image = product.MainImage;                     
                        basketVM.Price = product.Price;
                    }

                }

            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            return View(basketVMs);
        }
        public async Task<IActionResult> AddBasket(int? id)
        {
            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();

            Product product = products.FirstOrDefault(p => p.Id == id);

            if (id == null) { return BadRequest(); }

            if (product == null)
            {
                return NotFound();
            }


            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                List<BasketVM> basketVMs = new List<BasketVM>();

                BasketVM basketVM = new BasketVM
                {
                    Id = product.Id,
                    Image = product.MainImage,
                    Title = product.Title,
                    Price = product.Price,
                    Count = 1,
                    DiscountedPrice = product.DiscountedPrice
                };

                basketVMs.Add(basketVM);

                string strProducts = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", strProducts);

            }
            else
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                if (basketVMs.Exists(b => b.Id == id))
                {
                    basketVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    BasketVM basketVM = new BasketVM
                    {
                        Id = product.Id,
                        Image = product.MainImage,
                        Title = product.Title,
                        Price = product.Price,
                        Count = 1,
                        DiscountedPrice = product.DiscountedPrice

                    };

                    basketVMs.Add(basketVM);

                }

                if (User.Identity.IsAuthenticated)
                {
                    AppUser appUser = await _userManager.Users
                        .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());


                    if (appUser.Baskets.Any(b => b.ProductId == id))
                    {
                        appUser.Baskets.FirstOrDefault(b => b.ProductId == id).Count = basketVMs.FirstOrDefault(b => b.Id == id).Count;
                    }
                    else
                    {
                        Basket dbbasket = new Basket
                        {
                            ProductId = id,
                            Count = basketVMs.FirstOrDefault(b => b.Id == id).Count,

                        };

                        appUser.Baskets.Add(dbbasket);

                    }
                    await _context.SaveChangesAsync();

                }


                string strProducts = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", strProducts);

            }


            return Ok();
        }



        public async Task<IActionResult> GetBasket()
        {
            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]));
        }

        public async Task<IActionResult> PlusCount(int? id)
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            List<Product> products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            Product product = products.FirstOrDefault(p => p.Id == id);

            if (id == null)
            {
                return BadRequest();
            }

            if (!basketVMs.Any(b => b.Id == id))
            {
                return NotFound();
            }

            if (product.Count > basketVMs.Find(b => b.Id == id).Count)
            {

                basketVMs.FirstOrDefault(b => b.Id == id).Count += 1;

                string strProducts = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", strProducts);

            }

            return PartialView("_BasketPartialView", basketVMs);
        }

        public async Task<IActionResult> MinusCount(int? id)
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            if (id == null)
            {
                return BadRequest();
            }

            if (!basketVMs.Any(b => b.Id == id))
            {
                return NotFound();
            }

            if (basketVMs.FirstOrDefault(b => b.Id == id).Count > 1)
            {

                basketVMs.FirstOrDefault(b => b.Id == id).Count -= 1;

                string strProducts = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", strProducts);

            }

            return PartialView("_BasketPartialView", basketVMs);
        }

        public async Task<IActionResult> DeleteBasket(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (string.IsNullOrWhiteSpace(basket)) { return BadRequest(); }

            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                if (basketVMs.Exists(b => b.Id == id))
                {
                    BasketVM basketVM = basketVMs.Find(b => b.Id == id);
                    basketVMs.Remove(basketVM);
                    basket = JsonConvert.SerializeObject(basketVMs);
                    HttpContext.Response.Cookies.Append("basket", basket);
                }
                else
                {
                    return NotFound();
                }
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.DiscountedPrice = product.DiscountedPrice;
                    basketVM.Price = product.Price;
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                }
            }

            return PartialView("_BasketPartialView", basketVMs);
        }

        public IActionResult BasketCount()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return Json(0);
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            int count = basketVMs.Select(b=>b.Id).Distinct().Count();

            return Json(count);
        }
    }
}
