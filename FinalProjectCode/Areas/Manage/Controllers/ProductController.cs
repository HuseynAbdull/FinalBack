using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Extensions;
using FinalProjectCode.Helper;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ProductController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<Product> products = _context.Products.Where(p => p.IsDeleted == false);
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Genders =await _context.Genders.Where(g=>g.IsDeleted == false).ToListAsync();
            ViewBag.ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync();
            ViewBag.ProductTypes = await _context.ProductTypes.Where(p => p.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if(!await _context.ProductTypes.AnyAsync(p=>p.IsDeleted == false && p.Id == product.ProductTypeId))
            {
                ModelState.AddModelError("ProductTypeId", $"Daxil olunan ProductTypeId{product.ProductTypeId}yanlishdir");

            }

            if (!await _context.Genders.AnyAsync(p => p.IsDeleted == false && p.Id == product.GenderId))
            {
                ModelState.AddModelError("GenderId", $"Daxil olunan GenderId{product.GenderId}yanlishdir");

            }

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "MainFile  Yalniz JPG Formatda ola biler");
                    return View(product);
                }
                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "MainFile Yalniz 300Kb  ola biler");
                    return View(product);
                }
                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "photos", "products");
            }
            else
            {
                ModelState.AddModelError("MainFile", "MainFile File mutleqdir");
                return View(product);
            }

            if (product?.Files?.Count() <= 6)
            {
                if (product.Files != null && product.Files.Count() > 0)
                {
                    List<ProductImage> productImages = new List<ProductImage>();
                    foreach (IFormFile file in product.Files)
                    {
                        if (!file.CheckFileContentType("image/jpeg"))
                        {
                            ModelState.AddModelError("file", "Main File Yalniz JPG Formatda ola biler");
                            return View(product);
                        }
                        if (!file.CheckFileLength(300))
                        {
                            ModelState.AddModelError("file", "Main File Yalniz 300Kb  ola biler");
                            return View(product);
                        }
                        ProductImage productImage = new ProductImage()
                        {
                            Image = await file.CreateFileAsync(_env, "assets", "photos", "products"),
                            CreatedAt = DateTime.UtcNow.AddDays(4),
                            CreatedBy = "System"
                        };
                        productImages.Add(productImage);
                    }

                    product.ProductImages = productImages;
                    product.CreatedAt = DateTime.UtcNow.AddDays(4);
                    product.CreatedBy = "System";
                }
            }
            else
            {
                ModelState.AddModelError("Files", "max 6 shekil");
                return View(product);
            }


            product.Title= product.Title.Trim();
            product.CreatedBy = "System";
            product.CreatedAt = DateTime.UtcNow.AddDays(4);
            

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Detail(int? productId)
        {
            if (productId == null)
            {
                BadRequest();
            }
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p=>p.IsDeleted == false))
                .Where(p => p.IsDeleted == false).FirstOrDefaultAsync(p=>p.Id==productId && p.IsDeleted==false);
            if (product == null) NotFound(); 
            return View(product);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? productId)
        {
            if (productId == null)
            {
                BadRequest();
            }
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.IsDeleted == false))
                .Where(p => p.IsDeleted == false).FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted == false);
            if (product == null) NotFound();
            return View(product);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteProduct(int? productId)
        {
            if (productId == null)
            {
                BadRequest();
            }
            Product product = await _context.Products
                .Where(p => p.IsDeleted == false).FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted == false);
            if (product == null) NotFound();

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow.AddHours(4);
            product.DeletedBy = "system";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? productId)
        {
            if (productId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(t => t.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsDeleted == false);
            if (product == null) return NotFound();

            ViewBag.ProductTypes = await _context.ProductTypes
                .Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Genders = await _context.Genders.Where(b => b.IsDeleted == false).ToListAsync();


            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update( Product product)
        {
            ViewBag.ProductTypes = await _context.ProductTypes
            .Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Genders = await _context.Genders
            .Where(b => b.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if (product.Id == null) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p => p.ProductImages.Where(pImages => pImages.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == product.Id && c.IsDeleted == false);

            if (dbProduct == null) return NotFound();

            int canUpload = 6 - dbProduct.ProductImages.Count();
            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} Qeder sekil yukleye bilersiniz");
                return View(product);

            }
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("file", "Main File Yalniz JPG Formatda ola biler");
                        return View(product);
                    }
                    if (!file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("file", "Main File Yalniz 300Kb  ola biler");
                        return View(product);
                    }
                    ProductImage productImage = new ProductImage()
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "photos", "products"),
                        CreatedAt = DateTime.UtcNow.AddDays(4),
                        CreatedBy = "System"
                    };
                    productImages.Add(productImage);
                }

                dbProduct.ProductImages.AddRange(productImages);
            }
            
            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Formatda ola biler");
                    return View(product);
                }
                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300Kb  ola biler");
                    return View(product);
                }
                FileHelper.DeleteFile(dbProduct.MainImage, _env, "assets", "photos", "products");

                dbProduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "photos", "products");
            }

            


            if (product.Price != null) { dbProduct.Price = product.Price; }
            if (product.DiscountedPrice != null) { dbProduct.DiscountedPrice = product.DiscountedPrice; }
            if (product.Count != null) { dbProduct.Count = product.Count; }
            if (product.Description != null) { dbProduct.Description = product.Description; }
            if(product.Title != null) { dbProduct.Title = product.Title; }
           
            dbProduct.Title= product.Title.Trim();
            dbProduct.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbProduct.UpdatedBy = "ADMIN";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]

        public async Task<IActionResult> DeleteImg(int productId,int imageId)
        {
            if (productId == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(P => P.IsDeleted == false && P.Id == productId);

            if (product == null) return NotFound();

            if (product.ProductImages.Any(p => p.Id == imageId))
            {
                product.ProductImages.FirstOrDefault(product => product.Id == imageId).IsDeleted = true;
                await _context.SaveChangesAsync();

                FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(product => product.Id == imageId).Image, _env, "assets", "photos", "products");

            }
            else
            {
                return BadRequest();
            }
            List<ProductImage> productImages = product.ProductImages.Where(p => p.IsDeleted == false).ToList();



            return PartialView("_ProductImagePartial", productImages);
        }


    }
}
