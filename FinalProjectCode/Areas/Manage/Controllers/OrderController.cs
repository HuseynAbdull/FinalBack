using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Enums;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            IQueryable<Order> queries= _context.Orders
                .Include(o=>o.OrderItems);
            
            return View(queries);
        }

        [HttpGet]

        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();

            Order order = await _context.Orders
                .Include (o=>o.OrderItems.Where(oi=>oi.IsDeleted ==false))
                .ThenInclude(oi=>oi.Product)
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == id);

            if (order == null) return NotFound();

            return View(order);
                
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(Order order)
        {
            if (order == null) return BadRequest();

            if(order.Id <= 0) return BadRequest();
           
            Order dbOrder = await _context.Orders
                .Include(o => o.OrderItems.Where(oi => oi.IsDeleted == false))
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.IsDeleted == false && o.Id == order.Id);

            if (dbOrder == null) return NotFound();


            if (!ModelState.IsValid)
            {
                return View("Detail", dbOrder);
            }
            
            
            dbOrder.Status =order.Status;
            dbOrder.Comment= order.Comment;

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }


    }
}
