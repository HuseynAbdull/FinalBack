using FinalProjectCode.Models;
using FinalProjectCode.ViewModels.RegisterVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;

namespace FinalProjectCode.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse("husennikov@gmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "Test Email Confirm"
            };


            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync("husennikov@gmail.com", "oooomwecmdxuhqws");
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

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null) 
            {
                ModelState.AddModelError("", "Email və ya Şifrə Yanlişdir");
                return View(loginVM);
            }


            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser,loginVM.Password,loginVM.RemeindMe,true);

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email ve ya Şifrə Yanlishdir");
                return View(loginVM);
            }

            return RedirectToAction("index","home");
        }

        public IActionResult MyAccount()
        {
            return View();
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
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                Email = appUser.Email,
                Surname = appUser.Surname,
                Username = appUser.UserName,
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
