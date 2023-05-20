using FinalProjectCode.Areas.Manage.ViewModels.DashboardVMs;
using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Enums;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;

        public DashBoardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Order> orders = await _context.Orders
             .Include(o => o.OrderItems)
             .ToListAsync();

           List<ContactMe> contactMes =await _context.ContactMes.ToListAsync();

            ViewBag.YearlyRevenue = 0;
            foreach (var order in orders)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    ViewBag.YearlyRevenue += orderItem.Count * orderItem.Price;
                }


            }
            if (DateTime.UtcNow.AddHours(4).ToString("dd/MM") == "01.01")
            {
                ViewBag.YearlyRevenue = 0;
            }

            ViewBag.MountlyRevenue = 0;
            foreach (var order in orders)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    ViewBag.MountlyRevenue += orderItem.Count * orderItem.Price;
                }


            }
            if (DateTime.UtcNow.AddHours(4).ToString("dd") == "01")
            {
                ViewBag.MountlyRevenue = 0;
            }



            orders = orders.Where(o => o.Status == OrderType.Pending).OrderByDescending(o => o.CreatedAt).Take(20).ToList();


            DashboardVM dashboardVM = new DashboardVM
            {
                Order = orders,
                ContactMes = contactMes
            };

         

            return View(dashboardVM);
        }


    }
}
