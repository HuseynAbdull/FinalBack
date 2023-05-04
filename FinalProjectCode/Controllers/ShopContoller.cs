using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.ShopVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Globalization;

namespace FinalProjectCode.Controllers
{
    public class ShopController : Controller
    {

        private readonly AppDbContext _context;


        public ShopController(AppDbContext context)
        {
            _context = context;
        }
            
        public async Task<IActionResult> Index()   
        {

            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Products.Count() / 6);

            Products = Products.Skip(0).Take(6);


            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync(),
                Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync(),
            };


            return View(shopVM);
        }

        public async Task<IActionResult> GetFilteredProducts(int? genderid ,int? producttypeid,int pageindex = 1)
        {
            IEnumerable<Product> Products= await _context.Products.Where(p=>p.IsDeleted== false).ToListAsync();
            if(genderid != null)
            {
                Products = Products.Where(p => p.GenderId == genderid).ToList();
                ViewBag.GenderId = genderid;
            }
            
            if (producttypeid != null)
            {
                Products = Products.Where(t => t.ProductTypeId == producttypeid).ToList();
                ViewBag.ProductTypeId = producttypeid;
            }

            ViewBag.pageCount = (int)Math.Ceiling((decimal)Products.Count() / 6);

            ViewBag.pageIndex= pageindex;

            Products = Products.Skip((pageindex-1)*6).Take(6);


            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync(),
                Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync(),
            };

            return PartialView("_ShopPartialView", shopVM);
        }

        public async Task<IActionResult> Search(string search)
        {

            IEnumerable<Product> products = await _context.Products
                .Where(p => p.IsDeleted == false && 
                (p.Title.ToLower().Contains(search.ToLower()) || p.BrandName.ToLower().Contains(search.ToLower()))).ToListAsync();


            return PartialView("_SearchPartial", products);
        }


    }
}
