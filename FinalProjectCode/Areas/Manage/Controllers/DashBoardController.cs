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

        public IActionResult Index()
        {
            /*IQueryable<Order> orders = _context.Orders
               .Include(o => o.OrderItems);*/
            /*decimal yearlyEarnings = GetYearlyEarnings(); 
            decimal monthlyEarnings = GetMonthlyEarnings(); */

            /*ViewData["YearlyEarnings"] = yearlyEarnings.ToString("C"); 
            ViewData["MonthlyEarnings"] = monthlyEarnings.ToString("C");*/



            return View();
        }

     /*   private decimal GetYearlyEarnings()
        {
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;

            decimal yearlyEarnings = _context.Orders
                .Where(o => o.CreatedAt== currentYear && o.Status == OrderType.Accepted)
                .Sum(o => o.OrderItems.Sum(oi => oi.Price * oi.Count));

            return yearlyEarnings;
        }

        private decimal GetMonthlyEarnings()
        {
            DateTime currentDate = DateTime.Now;
            int currentYear = currentDate.Year;
            int currentMonth = currentDate.Month;

            decimal monthlyEarnings = _context.Orders
                .Where(o => o.CreatedAt.Year == currentYear && o.CreatedDate.Month == currentMonth && o.Status == OrderType.Accepted)
                .Sum(o => o.OrderItems.Sum(oi => oi.Price * oi.Count));

            return monthlyEarnings;
        }*/
  

    }
}
