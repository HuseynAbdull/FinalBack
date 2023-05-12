using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
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


        public async Task<IActionResult> DeleteDetail(int? productTypeId)
        {
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(g => g.IsDeleted == false).ToListAsync();


            if (productTypeId == null)
            {
                BadRequest();
            }

            if (!ProductTypes.Any(g => g.Id == productTypeId))
            {
                return NotFound();
            }

            ProductType productType = ProductTypes.FirstOrDefault(g => g.Id == productTypeId);

            return View(productType);
        }


        public async Task<IActionResult> Delete(int? productTypeId)
        {
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync();


            if (productTypeId == null)
            {
                BadRequest();
            }

            if (!ProductTypes.Any(g => g.Id == productTypeId))
            {
                return NotFound();
            }

            ProductType productType = ProductTypes.FirstOrDefault(g => g.Id == productTypeId);

            _context.ProductTypes.FirstOrDefault(c => c.Id == productTypeId).IsDeleted = true;

            IEnumerable<Product> products = await _context.Products.Where(p =>p.IsDeleted == false).ToListAsync();

            if (products.Any(p=>p.ProductTypeId ==productTypeId))
            {
                foreach (Product product in products)
                {
                    if (product.ProductTypeId == productTypeId)
                    {
                        product.IsDeleted = true;
                    }

                }
            }

            productType.DeletedAt = DateTime.UtcNow.AddHours(4);
            productType.DeletedBy = "System";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Detail(int? productTypeId)
        {
            IEnumerable<ProductType> ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync();


            if (productTypeId == null)
            {
                BadRequest();
            }

            if (!ProductTypes.Any(g => g.Id == productTypeId))
            {
                return NotFound();
            }

            ProductType productType = ProductTypes.FirstOrDefault(g => g.Id == productTypeId);

            return View(productType);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? productTypeId)
        {

            if (productTypeId == null) { return BadRequest(); }
            ProductType productType = await _context.ProductTypes.FirstOrDefaultAsync(c => c.Id == productTypeId && c.IsDeleted == false);
            ViewBag.productTypeId = productTypeId;
            if (productType == null) { return NotFound(); }

            return View(productType);

        }


        [HttpPost]
        public async Task<IActionResult> Update(ProductType productType)
        {
            
            ProductType dbproductType = await _context.ProductTypes.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == productType.Id);

        

            if (!ModelState.IsValid)
            {
                return View(productType);
            }
            if (productType.Id == null) { return BadRequest(); }




            if (dbproductType == null) { return NotFound(); }



            if (string.IsNullOrEmpty(productType.Name))
            {
                ModelState.AddModelError("", "Ad bos ola bilmez");
                return View(productType);
            }

            dbproductType.Name = productType.Name.Trim();
            dbproductType.UpdatedBy = "System";
            dbproductType.CreatedAt = DateTime.UtcNow.AddHours(4);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
