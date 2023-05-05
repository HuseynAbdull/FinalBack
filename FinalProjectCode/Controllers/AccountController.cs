using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.RegisterVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using FinalProjectCode.ViewModels;
using Microsoft.Extensions.Options;
using FinalProjectCode.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using FinalProjectCode.ViewModels.BasketVM;
using Newtonsoft.Json;

namespace FinalProjectCode.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly SmtpSetting _smtpSetting;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager,
            IOptions<SmtpSetting> smtpSetting,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _smtpSetting = smtpSetting.Value;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid) return View(registerVM);

            AppUser appUser = new AppUser 
            {
               Name = registerVM.Name,
               Surname = registerVM.Surname,
               Email = registerVM.Email,
               UserName = registerVM.Username
            };



            IdentityResult identityResult= await _userManager.CreateAsync(appUser,registerVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Member");
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("EmailConfirm", "Account", new {id=appUser.Id, token=token},
                HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_EmailConfirmPartial.cshtml");
            string templateContent = await System.IO.File.ReadAllTextAsync(fullpath);
            templateContent = templateContent.Replace("{{url}}", url);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = templateContent
            };


            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.Auto);

                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.Users.Include(u=>u.Baskets.Where(b=>b.IsDeleted == false))
                .FirstOrDefaultAsync(u=>u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());
            

            if (appUser == null) 
            {
                ModelState.AddModelError("", "Email və ya Şifrə Yanlişdir");
                return View(loginVM);
            }


            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser,loginVM.Password,loginVM.RemeindMe,true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("","Hesab blocklanib");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email ve ya Şifrə Yanlishdir");
                return View(loginVM);
            }

            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                if(appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    List<BasketVM> basketVMs = new List<BasketVM>();

                    foreach (Basket basket1 in appUser.Baskets)
                    {
                        BasketVM basketVM = new BasketVM
                        {
                            Id = (int)basket1.ProductId,
                            Count = basket1.Count,
                        };

                         basketVMs.Add(basketVM);
                    }

                    basket = JsonConvert.SerializeObject(basketVMs);

                    HttpContext.Response.Cookies.Append("basket",basket);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket","");
            }

            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> MyAccount()
        {
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                Email = appUser.Email,
                Surname = appUser.Surname,
                Username = appUser.UserName,
                Addresses = appUser.Addresses,
            };

            return View(profileVM);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
        
            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                Email = appUser.Email,
                Surname = appUser.Surname,
                Username = appUser.UserName,
                Addresses = appUser.Addresses,
            };

            return View(profileVM);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public async Task<IActionResult>Profile(ProfileVM profileVM)
        {
            if(!ModelState.IsValid)
            {
                return View(profileVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.Surname = profileVM.Surname;

            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }
            if (appUser.NormalizedUserName != profileVM.Username.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.Username;
            }
           

          IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded) 
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }

                return View(profileVM);
            }

            await _signInManager.SignInAsync(appUser, true);

            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser,profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword","Kohne Sifre Yanlisdir");
                    return View(profileVM);
                }

                if (profileVM.OldPassword == profileVM.Password)
                {
                    ModelState.AddModelError("Password", "Sifreler Eyni Ola Bilmez");
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult = await _userManager.ResetPasswordAsync(appUser,token,profileVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }

                    return View(profileVM);
                }

            }

            return RedirectToAction("index", "home");
        }


        [HttpGet]
        public async Task<IActionResult> EmailConfirm(string id,string token)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

          IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded) 
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(appUser, false);

            return RedirectToAction("index", "home");
        }


        [HttpPost]
        [Authorize(Roles ="Member")]

        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
               .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses
            };


            if (!ModelState.IsValid)
            {
                return View(nameof(Profile),profileVM);
            }

            if(address.IsMain == true && appUser.Addresses != null &&appUser.Addresses.Count() > 0 && appUser.Addresses.Any(u=>u.IsMain == true))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain == true).IsMain = false;

            }

            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.Surname}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Profile));
        }

        #region Create Role And SuperAdmin
        /*      [HttpGet]
      public async Task<IActionResult> CreateRole()
      {
          await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
          await _roleManager.CreateAsync(new IdentityRole("Admin"));
          await _roleManager.CreateAsync(new IdentityRole("Member"));

          return Content("Ugurlu");
      }
*/
        /*    [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            AppUser appUser = new AppUser
            {
                Name = "Super",
                Surname = "Admin",
                UserName = "SuperAdmin",
                Email = "superadmin@gmail.com"
            };
            await _userManager.CreateAsync(appUser,"SuperAdmin123");
            await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

            return Content("Ugurlu");
        }
*/
        #endregion

    }
}
