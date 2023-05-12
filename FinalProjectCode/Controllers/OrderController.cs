using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.BasketVM;
using FinalProjectCode.ViewModels.OrderVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinalProjectCode.Controllers
{

    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser>_userManager;
        private readonly AppDbContext _context;

        public OrderController(UserManager<AppUser> userManager,AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string coockie = HttpContext.Request.Cookies["basket"];


            if (string.IsNullOrWhiteSpace(coockie))
            {
                return RedirectToAction("Index","Product");
            }


            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                basketVM.Price =product.Price;
                basketVM.DiscountedPrice = product.DiscountedPrice;
                basketVM.Title =product.Title;
           
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a=>a.IsMain && a.IsDeleted == false))
                .FirstOrDefaultAsync(u=>u.UserName == User.Identity.Name);

            Order order = new Order
            {
                Name =appUser.Name,
                SurName=appUser.Surname,
                Email=appUser.Email,
                Phone=appUser.PhoneNumber,
                AddressLine =appUser.Addresses?.FirstOrDefault()?.AddressLine,
                City = appUser.Addresses?.FirstOrDefault()?.City,
                Country =appUser.Addresses?.FirstOrDefault()?.Country,
                State =appUser.Addresses?.FirstOrDefault()?.State,
                PostalCode = appUser.Addresses?.FirstOrDefault()?.PostalCode
            };

           

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs,
            };


            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u=>u.Orders)
                .Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
                .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            string coockie = HttpContext.Request.Cookies["basket"];


            if (string.IsNullOrWhiteSpace(coockie))
            {
                return RedirectToAction("Index", "Shop");
            }


            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                basketVM.Price = product.Price;
                basketVM.DiscountedPrice = product.DiscountedPrice;
                basketVM.Image = product.MainImage;
                basketVM.Title = product.Title;

            }

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs,
            };
            if (orderVM.Order == null)
            {
                orderVM.Order = new Order();
            }

        
            if (!ModelState.IsValid)
            {
                return View();
            }

            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (BasketVM basketVM in basketVMs)
            {
                OrderItem orderItem = new OrderItem
                {
                    Count = basketVM.Count,
                    ProductId = basketVM.Id,
                    Price = basketVM.Price,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.Surname}",

                };

                orderItems.Add(orderItem);
            }

            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;
            }

            HttpContext.Response.Cookies.Append("basket", "");


            order.UserId = appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            order.OrderItems = orderItems;
            order.No = appUser.Orders != null && appUser.Orders.Count() > 0 ? appUser.Orders.Last().No + 1 : 1;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            TempData["ToasterMessage4"] = "Order Placed Successfully!";
            return RedirectToAction("index", "home");
        }
    }
}
