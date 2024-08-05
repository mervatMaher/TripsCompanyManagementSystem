using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TripsCompanySystemDbContext _context;


        public HomeController(ILogger<HomeController> logger, TripsCompanySystemDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var Compaines = _context.Companies.Include(c => c.Ratings).ToList();

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
