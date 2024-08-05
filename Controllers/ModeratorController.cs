using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class ModeratorController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ModeratorController (TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }


        // Add Users In Moderator Role
        public IActionResult AddUserInModerator()
        {
            var Register = new RegistrationViewModel();
            return View(Register);
        }

        public async Task<IActionResult> SaveUserInModerator(RegistrationViewModel registModel)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }



            if (!_context.Users.Any(u => u.FullName == registModel.FullName && u.UserName == registModel.UserName)) {

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
                     await _userManager.AddToRoleAsync(user, "Moderator");
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
              }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            else
            {
                ModelState.AddModelError(string.Empty, "A user with the same Full Name and UserName already exists.");
            }
            return View("AddUserInModerator", registModel);
        }

        public IActionResult CreateTrips ()
        {
            return View();

        }
    }
}
