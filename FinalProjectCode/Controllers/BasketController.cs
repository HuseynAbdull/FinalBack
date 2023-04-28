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

        public BasketController(AppDbContext context)
        {
            _context = context;
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

            var itemToRemove = basketVMs.SingleOrDefault(b => b.Id == id);
            if (itemToRemove != null)
            {
                basketVMs.Remove(itemToRemove);
            }

            if (basketVMs.Count == 0)
            {
                HttpContext.Response.Cookies.Delete("basket");
            }
            else
            {
                string jsonBasket = JsonConvert.SerializeObject(basketVMs);
                HttpContext.Response.Cookies.Append("basket", jsonBasket);
            }

            return PartialView("_BasketPartialView", basketVMs);
        }


    }
}
