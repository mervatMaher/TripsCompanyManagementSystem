using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TripsCompanySystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AdminController (TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Add Users In Admin Role
        public IActionResult AddUserInAdmin()
        {
            var Register = new RegistrationViewModel();
            return View(Register);
        }
        public async Task<IActionResult> SaveUserInAdmin([FromForm] RegistrationViewModel registModel)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
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

            if (!_context.Users.Any(u => u.FullName == registModel.FullName && u.UserName == registModel.UserName))
            {
                var result = await _userManager.CreateAsync(user, registModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

      
            return View("AddUserInAdmin", registModel);
        }

        // show All the compaines First 
        public IActionResult AllCompaines()
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            ViewBag.User = user;
            var compaines = _context.Companies.ToList();
            
            return View(compaines);
        }

        // AddCompaines, Edit, Delete 
        public IActionResult CreateCompany()
        {
            var addCompany = new AddCompanyViewModel();
            return View(addCompany);
        }

        public async Task<IActionResult> SaveCompany([FromForm] AddCompanyViewModel companyModel)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");

            if (! Directory.Exists(uploadDirectory)) {
                Directory.CreateDirectory(uploadDirectory);
            };

            var company = new Company
            {
                Name = companyModel.CompanyName,
            };

            if (companyModel.CompanyImage != null)
            {

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + companyModel.CompanyImage.FileName;

                var pathCompanyImage = Path.Combine(uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(pathCompanyImage, FileMode.Create))
                {
                    await companyModel.CompanyImage.CopyToAsync(stream);
                }

                company.ImageUrl = uniqueFileName;
                Console.WriteLine($"Saved image to: {pathCompanyImage}");
                Console.WriteLine($"Image URL set to: {company.ImageUrl}");
            }

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return RedirectToAction("AllCompaines", "Admin", company);
        }

        public IActionResult DeleteCompany (int id)
        {
            var company = _context.Companies.Find(id);
         
            _context.Companies.Remove(company);
            _context.SaveChanges();
            return RedirectToAction("AllCompaines","Admin");
        }

        public IActionResult CompanyDetailsAdminView(int id)
        {
            var company = _context.Companies.Include(c => c.Users).Where(c => c.Id == id)
                .Select(c => new
                {
                    CompanyId = c.Id,
                    CompanyName = c.Name,
                    companyImage = c.ImageUrl,
                    Moderators = _context.Users.Where(u => u.CompanyId == c.Id)
                    .Select(m => new
                    {
                        ModeratorId = m.Id,
                        ModeratorName = m.FullName,
                        ModeratorImage = m.MainImage
                    }).ToList()
                }).FirstOrDefault();

            ViewBag.Company = company;
            return View();
        }
        
        // Remove Moderator From Company
        public IActionResult DeleteModerator(string id)
        {
            var companyId = _context.Users.Where(u => u.Id == id).Select(u => u.CompanyId).FirstOrDefault();
            var moderator = _context.Users.Find(id);
            if (moderator != null) {
                _context.Users.Remove(moderator);
            }
           
            _context.SaveChanges();
            return RedirectToAction("CompanyDetailsAdminView", "Admin", new {id = companyId });
        }

        public IActionResult EditView (int id)
        {
            var company = _context.Companies.Find(id);
            return View(company);
        }

        public async Task<IActionResult> EditCompany (int id, EditCompanyViewModel editCompanycompanyModel)
        {
            var rootDirectoy = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectoy, "wwwroot", "Images");

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var company = _context.Companies.Find (id);
            if (company != null)
            {
                company.Name = editCompanycompanyModel.CompanyName;

                if (editCompanycompanyModel.CompanyImage != null)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + editCompanycompanyModel.CompanyImage.FileName;
                    var filePath = Path.Combine(uploadDirectory, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await editCompanycompanyModel.CompanyImage.CopyToAsync(stream);
                    }
                    company.ImageUrl = uniqueFileName;
                }
                _context.SaveChanges();
            }
       

            return RedirectToAction("AllCompaines", "Admin");
        }


        //Add ModeratoresInCompany 
        public IActionResult AddModeratorInCompany(int id)
        {
           var Moderator = new ModeratorViewModel();
            var companyId = _context.Companies.Where(c => c.Id == id).Select(c => c.Id).FirstOrDefault();
            var CompanyName = _context.Companies.Where(c => c.Id == id).Select(c => c.Name).FirstOrDefault();
            var company = _context.Companies.Find(id);

            ViewData["Company"] = company;

            ViewData["CompanyId"] = companyId;
            ViewData["CompanyName"] = CompanyName;
            return View(Moderator);
        }

        public async Task<IActionResult> SaveModeratorInCompany (int Id ,[FromForm] ModeratorViewModel moderatorView)
        {

            var rootDirectory = Directory.GetCurrentDirectory();
            var uploadDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var Moderator = new ApplicationUser
            {
                FullName = moderatorView.FullName,
                Email = moderatorView.Email,
                UserName = moderatorView.UserName,
                PhoneNumber = moderatorView.PhoneNumber,
                CompanyId = Id,
            };

            if (moderatorView.MainImage != null)
            {

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + moderatorView.MainImage.FileName;

                var pathCompanyImage = Path.Combine(uploadDirectory, uniqueFileName);

                using (var stream = new FileStream(pathCompanyImage, FileMode.Create))
                {
                    await moderatorView.MainImage.CopyToAsync(stream);
                }

                Moderator.MainImage = uniqueFileName;
                Console.WriteLine($"Saved image to: {pathCompanyImage}");
                Console.WriteLine($"Image URL set to: {Moderator.MainImage}");
            }
            else
            {
                Console.WriteLine("No image file uploaded.");
            }

            if (moderatorView != null)
            {
                    var result = await _userManager.CreateAsync(Moderator, moderatorView.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(Moderator, "Moderator");
                        return RedirectToAction("AllCompaines", "Admin");
                    }

            }

            return RedirectToAction("AddModeratorInCompany", "Admin");
        }


    }
}
