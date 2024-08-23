using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        public HomeController(TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<HomeController> logger)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var topCompanies = _context.Companies
              .Include(c => c.Ratings)
             .OrderByDescending(c => c.Ratings.Any() ? c.Ratings.Average(r => r.Score) : 0)
             .Take(6)
             .ToList();
            return View(topCompanies);
        }

        [HttpGet]
         public IActionResult HomeView()
         {
            var userId = _userManager.GetUserId(User);
            var Compaines = _context.Companies.Include(c => c.Ratings).ToList();

            var UserName = _context.Users.Where( u => u.Id == userId).Select(u => u.FullName).FirstOrDefault();
            ViewBag.UserName = UserName;
            return View(Compaines);
         }
    
        public IActionResult Search (string search)
        {
            var SearchResults = new List<object>();

            var Companies = _context.Companies.Where(c => c.Name.Contains(search)).ToList();
            SearchResults.AddRange(Companies);



            var Trips = _context.Trips.Include(t => t.Company)
                .Where(t => t.Name.Contains(search) ||
                t.DestinationFrom.Contains(search) ||
                t.DestinationTo.Contains(search)
                ).ToList();

            SearchResults.AddRange(Trips);

            return View("_SearchResult", SearchResults);
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
