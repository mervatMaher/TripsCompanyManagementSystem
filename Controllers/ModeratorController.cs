using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.ComponentModel.Design;
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

        public IActionResult CompanyTrips ()
        {
            var userId = _userManager.GetUserId(User);
            var companyTrips = _context.Users.Include(c => c.Company).Where(u => u.Id == userId)
                .Select(u => new
                {
                    CompanyId = u.CompanyId,
                    CompanyName = u.Company.Name,
                    CompanyImage = u.Company.ImageUrl,
                    UserId = userId,
                    UserName = u.FullName,
                    Trips = _context.Trips.Where( t => t.CompanyId == u.CompanyId)
                    .Select(t => new
                    {
                        TripId = t.Id,
                        TripName = t.Name,
                        TripImage = t.MainImage
                    }).ToList(),

                })
                .FirstOrDefault();

            ViewBag.CompanyTrips = companyTrips;
            return View();

        }
        public IActionResult CreateTrip (int id)
        {
            var userId = _userManager.GetUserId(User);
            var user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();

            var company = _context.Companies.Where(c => c.Id == id).FirstOrDefault();


            ViewBag.Company = company;
            ViewBag.User = user;

            var ModeratorCreateTrip = new ModeratorCreateTripViewModel();
            return View(ModeratorCreateTrip);
        }

        public async Task<IActionResult> SaveTrip(int CompanyId ,[FromForm] ModeratorCreateTripViewModel createTripViewModel)
        {
            var userId = _userManager.GetUserId(User);

            var rootDirectory = Directory.GetCurrentDirectory();
            var ImagesDirectory = Path.Combine(rootDirectory, "wwwroot", "Images");

            if (!Directory.Exists(ImagesDirectory))
            {
                Directory.CreateDirectory(ImagesDirectory);
            }

            var trip = new Trip
            {
                Name = createTripViewModel.Name,
                Description = createTripViewModel.Description,
                TripStartDate = createTripViewModel.TripStartDate,
                TripEndDate = createTripViewModel.TripEndDate,
                DestinationFrom = createTripViewModel.DestinationFrom,
                DestinationTo = createTripViewModel.DestinationTo,
                Discount = createTripViewModel.Discount,
                TripPrice = createTripViewModel.TripPrice,
                TripPriceAfterDiscount = createTripViewModel.TripPriceAfterDiscount,
                CompanyId = CompanyId,
            };
            if (trip != null)
            {
                if (createTripViewModel.MainImage != null)
                {
                    var UniqePath = Guid.NewGuid().ToString() + "_" + createTripViewModel.MainImage.FileName;
                    var pathTripImage = Path.Combine(ImagesDirectory, UniqePath);

                    using (var sream = new FileStream(pathTripImage, FileMode.Create))
                    {
                        await createTripViewModel.MainImage.CopyToAsync(sream);
                    }
                    trip.MainImage = UniqePath;
                }
                _context.Trips.Add(trip);
                _context.SaveChanges();

                return RedirectToAction("CompanyTrips", "Moderator");
            }

            return RedirectToAction("CreateTrip" , "Moderator", new {id = CompanyId});
        }
        public IActionResult EditTripView (int TripId)
        {
            var trip = _context.Trips.Find(TripId);
            return View(trip);
        }

        public async Task<IActionResult> SaveEditTripView(int TripId,int CompanyId ,[FromForm] ModeratorEditTripViewModel editTripModel)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var ImagesPath = Path.Combine(rootDirectory, "wwwroot", "Images");
            

            if (Directory.Exists(ImagesPath))
            {
                Directory.CreateDirectory(ImagesPath);
            }

            var trip =  await _context.Trips.FindAsync(TripId);
            if (trip != null)
            {
                trip.Name = editTripModel.Name;
                trip.Description = editTripModel.Description;
                trip.TripStartDate = editTripModel.TripStartDate;
                trip.TripEndDate = editTripModel.TripEndDate;
                trip.DestinationFrom = editTripModel.DestinationFrom;
                trip.DestinationTo = editTripModel.DestinationTo;
                trip.Discount = editTripModel.Discount;
                trip.TripPrice = editTripModel.TripPrice;
                trip.TripPriceAfterDiscount = editTripModel.TripPriceAfterDiscount;
                trip.CompanyId = CompanyId;

                if (editTripModel.MainImage != null)
                {
                    var uniqePath = Guid.NewGuid().ToString() + "_" + editTripModel.MainImage.FileName;
                    var filePath = Path.Combine(ImagesPath, uniqePath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await editTripModel.MainImage.CopyToAsync(stream);
                    }
                    trip.MainImage = uniqePath;
                }
                _context.Update(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction("CompanyTrips", "Moderator");

            }

            return RedirectToAction("EditTripView", "Moderator", new { TripId = TripId });
        }


        public IActionResult DeleteTrip(int TripId)
        {
            var trip = _context.Trips.Find(TripId);
            if (trip != null)
            {
                _context.Trips.Remove(trip);
                _context.SaveChanges();
            }

            return RedirectToAction("CompanyTrips", "Moderator");
        }
    }
}
