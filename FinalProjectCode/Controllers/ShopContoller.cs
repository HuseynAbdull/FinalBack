using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.ShopVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Data;
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
            
        public async Task<IActionResult> Index(int? genderid)   
        {

            IEnumerable<Product> Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();
            if(genderid != null)
            {
                Products = Products.Where(p=>p.GenderId == genderid).ToList();
            }
            ViewBag.genderid = genderid;
            ViewBag.pageCount = (int)Math.Ceiling((decimal)Products.Count() / 6);
            ViewBag.pageIndex = 1;


            Products = Products.Skip(0).Take(6);


            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync(),
                Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync(),
            };


            return View(shopVM);
        }

      
        public async Task<IActionResult> FilterProduct(int? genderid, int? producttypeid, int pageindex = 1, int sortid = 1, string range = "400")
        {
            IEnumerable<Product> productsQuery = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync();

            if (genderid != null)
            {
                productsQuery = productsQuery.Where(p => p.GenderId == genderid);
                ViewBag.genderid = genderid;
            }

            if (producttypeid != null)
            {
                productsQuery = productsQuery.Where(p => p.ProductTypeId == producttypeid);
                ViewBag.ProductTypeId = producttypeid;
            }

            if (!string.IsNullOrEmpty(range))
            {
                double rangeValue;
                if (double.TryParse(range, out rangeValue))
                {
                    productsQuery = productsQuery.Where(p => p.DiscountedPrice <= rangeValue);
                }
            }

            int pageSize = 6;
            int totalItems = productsQuery.Count();
            int pageCount = (int)Math.Ceiling((decimal)totalItems / pageSize);

            ViewBag.pageCount = pageCount;
            ViewBag.pageIndex = pageindex;
            ViewBag.range = range;

            List<Product> products = productsQuery.OrderBy(p => p.Title).Skip((pageindex - 1) * pageSize).Take(pageSize).ToList();

            ShopVM shopVM = new ShopVM
            {
                Products = products,
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
