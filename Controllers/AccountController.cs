using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

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
        public IActionResult Register()
        {
            var Register = new RegistrationModel();
            return View(Register);
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRegistration( /*[FromForm]*/ RegistrationModel registModel)
        {
            if(!ModelState.IsValid ) {

                _logger.LogWarning("Invalid model state for registration.");

            }

            var rolesName = new[] {"Admin", "Moderator", "Visitor" };
            foreach(var role in rolesName)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                  var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
    

                    if(!roleResult.Succeeded)
                    {
                        _logger.LogError("Failed To create role {Role}. Errors : {Errors}"
                            , role,
                            string.Join(" , ", roleResult.Errors.Select(e => e.Description)));

                        return StatusCode(500, "internal server error ");
                    };
                    
                }
            }

            var User = new ApplicationUser
            {
                FullName = registModel.FullName,
                UserName = registModel.UserName,
                Email = registModel.Email,
                PhoneNumber = registModel.PhoneNum,
                //MainImage = registModel.Image 

            };

            var result = await _userManager.CreateAsync(User, registModel.Password);
            await _context.SaveChangesAsync();


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(User, "Visitor");
                await _signInManager.SignInAsync(User, isPersistent: registModel.RememberMe);

                _logger.LogInformation("User {UserName} registered successfully.", User.UserName);

                return RedirectToAction("Index", "Home");
            }

            await _context.SaveChangesAsync();

            return View("Register", registModel); 
        }
    }
}
