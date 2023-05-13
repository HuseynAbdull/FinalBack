using FinalProjectCode.Areas.Manage.ViewModels.UserVMs;
using FinalProjectCode.DataAccessLayer;
using FinalProjectCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectCode.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(UserManager<AppUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            List<UserVM> query = await _userManager.Users
                .Where(u => u.UserName != User.Identity.Name)
                .Select(x => new UserVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    SurName = x.Surname,
                    Email = x.Email,
                    UserName = x.UserName,
                })
                .ToListAsync();

                foreach (var item in query)
                {
                    var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == item.Id);
                    string roleId = userRole != null ? userRole.RoleId : null;
                    var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
                    string roleName = role != null ? role.Name : null;
                    
                    item.RoleName = roleName;
                }

                return View(query.AsQueryable());
        }


        [HttpGet]
        public async Task<IActionResult> ChangeRole(string? id)
        {
            if(string.IsNullOrEmpty(id)) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;

            UserChangeRoleVM userChangeRoleVM = new UserChangeRoleVM
            {
                UserId = user.Id,
                RoleId = roleId
            };

            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();

            return View(userChangeRoleVM);
        }

        [HttpPost]

        public async Task<IActionResult> ChangeRole(UserChangeRoleVM userChangeRoleVM)
        {
            if(!ModelState.IsValid) return View(userChangeRoleVM);

            if (string.IsNullOrEmpty(userChangeRoleVM.UserId)) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(userChangeRoleVM.UserId);

            if (user == null) return NotFound();

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userChangeRoleVM.UserId).RoleId;
            string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            string newRoleName = _roleManager.Roles.FirstOrDefault(c => c.Name != "SuperAdmin" && c.Id == userChangeRoleVM.RoleId).Name;

            await _userManager.RemoveFromRoleAsync(user, roleName);
            await _userManager.AddToRoleAsync(user, newRoleName);
            


            return RedirectToAction(nameof(Index));
        }
    }
}

