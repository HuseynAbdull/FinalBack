using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GenderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genders
                .Where(c => c.IsDeleted == false).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Gender gender =new Gender();
            return View(gender);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Gender gender)
        {
            if (gender.File?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("File", "Ancaq jpeg");
                return View();
            }

            if((gender.File?.Length / 1024) > 300) 
            {
                ModelState.AddModelError("File", "Olchu maksimum 300 kb");
                return View();
            }

            if (gender.File != null)
            {
                
                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{gender.File.FileName}";
                string filePath = Path.Combine(_env.WebRootPath, "assets", "photos", "gender",fileName);
                gender.Image = fileName;

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await gender.File.CopyToAsync(stream);
                }
            }

            if (string.IsNullOrEmpty(gender.Name))
            {
                ModelState.AddModelError("", "Ad bos ola bilmez");
                return View(gender);
            }

            IEnumerable<Gender> genders = await _context.Genders.Where(p => p.IsDeleted == false).ToListAsync();
            if (genders.Any(g =>g.Name.ToLower().ToString().Trim() == gender.Name.ToLower().ToString().Trim()))
            {
                ModelState.AddModelError("", "Gender artiq var.Eyni Ola bilmez");
                return View(gender);
            }

            gender.CreatedAt = DateTime.UtcNow.AddHours(4);
            gender.CreatedBy = "System";
            gender.Name = gender.Name.ToLower().ToString().Trim();


            await _context.Genders.AddAsync(gender);
            await _context.SaveChangesAsync();


            return View();
        }

        public async Task<IActionResult> DeleteDetail(int? genderId)
        {
            IEnumerable<Gender> Genders = await _context.Genders.Where(g=>g.IsDeleted == false).ToListAsync();


            if (genderId == null)
            {
                BadRequest();
            }

            if (!Genders.Any(g=>g.Id == genderId))
            {
                return NotFound();
            }

            Gender gender =Genders.FirstOrDefault(g=>g.Id==genderId);

            return View(gender);
        }

        public async Task<IActionResult> Delete(int? genderId)
        {
            if (genderId == null) return NotFound();
            Gender Genders = await _context.Genders.Include(g => g.Products).Where(g => g.IsDeleted == false  && g.Id == genderId).FirstOrDefaultAsync();


            if (Genders == null) return BadRequest();

            foreach (Product product in Genders.Products)
            {
                product.IsDeleted = true;
            }

            Genders.IsDeleted = true;


            Genders.DeletedAt = DateTime.UtcNow.AddHours(4);
            Genders.DeletedBy = "System";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
           
        }


        public async Task<IActionResult> Detail(int?genderId)
        {
            IEnumerable<Gender> Genders = await _context.Genders.Where(g => g.IsDeleted == false).ToListAsync();


            if (genderId == null)
            {
                BadRequest();
            }

            if (!Genders.Any(g => g.Id == genderId))
            {
                return NotFound();
            }

            Gender gender = Genders.FirstOrDefault(g => g.Id == genderId);

            return View(gender);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int? genderId)
        {

            if (genderId == null) { return BadRequest(); }
            Gender gender = await _context.Genders.FirstOrDefaultAsync(c => c.Id == genderId && c.IsDeleted == false);
            ViewBag.genderId = genderId;
            if (gender == null) { return NotFound(); }

            return View(gender);
           
        }

        [HttpPost]
        public async Task<IActionResult> Update(Gender gender)
        {
            if (gender.File?.ContentType != "image/jpeg")
            {
                ModelState.AddModelError("File", "Ancaq jpeg");
                return View();
            }

            if ((gender.File?.Length / 1024) > 300)
            {
                ModelState.AddModelError("File", "Olchu maksimum 300 kb");
                return View();
            }
            Gender dbgender = await _context.Genders.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == gender.Id);

            if (gender.File != null)
            {

                string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-{Guid.NewGuid().ToString()}-{gender.File.FileName}";
                string filePath = Path.Combine(_env.WebRootPath, "assets", "photos", "gender", fileName);
                dbgender.Image = fileName;

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await gender.File.CopyToAsync(stream);
                }
            }



            if (!ModelState.IsValid)
            {
                return View(gender);
            }
            if (gender.Id == null) { return BadRequest(); }
           

           

            if (dbgender == null) { return NotFound(); }

         

             if (string.IsNullOrEmpty(gender.Name))
            {
                ModelState.AddModelError("", "Ad bos ola bilmez");
                return View(gender);
            }

            dbgender.Name = gender.Name.Trim();
            dbgender.UpdatedBy = "System";
            dbgender.CreatedAt = DateTime.UtcNow.AddHours(4);


            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        


    }
}
