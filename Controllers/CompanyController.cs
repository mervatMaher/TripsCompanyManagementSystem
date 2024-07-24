using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        public CompanyController(TripsCompanySystemDbContext context)
        {
            _context = context;
        }
        public IActionResult Companies()
        {
            var Compaines = _context.Companies.ToList();
            return View(Compaines);
        }

        public IActionResult CompanyDetail(int id)
        {
            var CompanyDetails = _context.Companies.Include(c => c.Trips)
                .Include(c => c.Ratings).Where(c => c.Id == id)
                .Select(c => new CompanyDetailModel
                {
                    Id = c.Id,
                    ImageUrl = c.ImageUrl,
                    Name = c.Name,
                    NormalizedAverageRating = (c.AverageRating ?? 0.0) / 100.0 * 5.0,
                    Trips = c.Trips
                    .Select(t => new TripDetailModel
                    {

                       Id = t.Id,
                       MainImage = t.MainImage,
                       Name = t.Name,
                       Description = t.Description
 
                    }).ToList()
                }).FirstOrDefault();


            if (CompanyDetails == null)
            {
                return NotFound();
            }

            return View(CompanyDetails);

        }

        public IActionResult TripDetail(int id)
        {
            var Trip = _context.Trips.Find(id);
            return View(Trip);
        }



    }
}
