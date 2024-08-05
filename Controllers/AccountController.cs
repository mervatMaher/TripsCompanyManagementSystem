using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace TripsCompanySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController( TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public JsonResult IsLoggedIn ()
        {
            var isLoggedIn = User.Identity.IsAuthenticated;
            return Json(new { isLoggedIn });
        }
        public IActionResult LogIn()
        {
            var Login = new LogInViewModel();
            return View(Login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLogIn(LogInViewModel logIn)
        {

            var user = await _userManager.FindByEmailAsync(logIn.Email);
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, logIn.Password);

                if (passwordCheck)
                {
                    
                        var result = await _signInManager.PasswordSignInAsync(user.UserName, logIn.Password, logIn.RememberMe, lockoutOnFailure: false);

                        if (result.Succeeded)
                        {

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Email or Password incorrect");
                        }
                }
                else
                {
                    ModelState.AddModelError("", "Email or Password incorrect");
                }
            }
            else
            {
                ModelState.AddModelError("", "Email or Password incorrect");
            }

            return View("LogIn", logIn);

        }
        public IActionResult Register()
        {
            var Register = new RegistrationViewModel();
            return View(Register);
        }

        [HttpPost] 
        public async Task<IActionResult> SaveRegistration([FromForm] RegistrationViewModel registModel)
        {

            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var RoleNames = new[] { "Admin", "Moderator", "Visitor" };
            foreach (var roleName in RoleNames)
            {
                if (!_context.Roles.Any(r => r.Name == roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var user = new ApplicationUser
            {
                FullName = registModel.FullName,
                Email = registModel.Email,
                UserName = registModel.UserName,
                PhoneNumber = registModel.PhoneNumber,
            };

            if (registModel.Image != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + registModel.Image.FileName;
                var pathCompanyImage = Path.Combine(uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(pathCompanyImage, FileMode.Create))
                {
                    await registModel.Image.CopyToAsync(stream);
                }

                user.MainImage = uniqueFileName;
            }


            var result = await _userManager.CreateAsync(user, registModel.Password);
             

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Visitor");
                await _signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Register", registModel);
        }

        public IActionResult VerifyEmail()  
        {
            var verifyEmail = new VerifyViewModel();
            return View(verifyEmail);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveVerifyEmail(VerifyViewModel verifyModel)
        {
            var user = await _userManager.FindByEmailAsync(verifyModel.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Something is Wrong");
                return View("VerifyEmail", verifyModel);
            } else
            {
                return RedirectToAction("ChangePassword", "Account", new {Email = verifyModel.Email});
            }
        }

        public IActionResult ChangePassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            var changePassword = new ChangePasswordViewModel();
            changePassword.Email = Email;

            return View(changePassword);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChangePassword(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(changePassword.Email);
                if (user != null)
                {
                    var result = await _userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddPasswordAsync(user, changePassword.NewPassword);
                        return RedirectToAction("LogIn", "Account"); 

                    } else
                    {
                        foreach(var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "EmailNotValid");
                }

            }
           
            return View("ChangePassword", changePassword);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult ShowProfile(string id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        public IActionResult EditProfile (string id)
        {
            var user = _context.Users.Find(id);
            return View(user);
        }

        public async Task<IActionResult> SaveEditProfile (string id ,EditProfileViewModel editProfileView)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var user = _context.Users.Find(id);
            if (user != null) {
                user.FullName = editProfileView.FullName;
                user.UserName = editProfileView.UserName;
                user.Email = editProfileView.Email;
                user.PhoneNumber = editProfileView.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(editProfileView.Password))
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, editProfileView.Password);
                
            }

            if (editProfileView.Image != null)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + editProfileView.Image.FileName;
                var pathCompanyImage = Path.Combine(uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(pathCompanyImage, FileMode.Create))
                {
                    await editProfileView.Image.CopyToAsync(stream);
                }

                user.MainImage = uniqueFileName;
            }
            _context.Update(user);
            _context.SaveChanges();

            return RedirectToAction("ShowProfile", "Account" ,new {id = user.Id});
        }

        
    }
}
