using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        public CompanyController(TripsCompanySystemDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
        }
        public IActionResult Companies()
        {
            var Compaines = _context.Companies.ToList();
            return View(Compaines);
        }

        public IActionResult CompanyDetail(int id)
        {
            var userId = _userManager.GetUserId(User);
            var UserRate = _context.Ratings.Where(r => r.CompanyId == id && r.UserId == userId).FirstOrDefault();
            var averageRate = _context.Trips.Where(t => t.CompanyId == id).Select(t => t.AverageRating).FirstOrDefault();


            var CompanyDetails = _context.Companies.Include(c => c.Trips)
                .Include(c => c.Ratings).Where(c => c.Id == id)
                .Select(c => new CompanyDetailViewModel
                {
                    Id = c.Id,
                    ImageUrl = c.ImageUrl,
                    Name = c.Name,
                    UserRating = c.Ratings
                        .Where(r => r.UserId == userId)
                        .Select(r => r.Score)
                        .FirstOrDefault(),
                    Trips = _context.Trips.Include(t=> t.Ratings).Where(t => t.CompanyId == c.Id)
                    .Select(t => new TripDetailViewModel
                    {

                        Id = t.Id,
                        MainImage = t.MainImage,
                        Name = t.Name,
                        Description = t.Description,
                        AverageRating = t.AverageRating


                    }).ToList()
                }).FirstOrDefault();


            if (CompanyDetails == null)
            {
                return NotFound();
            }

            ViewData["UserRate"] = UserRate;
            ViewData["AverageRate"] = averageRate;

            return View(CompanyDetails);

        }
        public IActionResult CompanyRating(CompanyRatingViewModel ratingModel)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            var existingRate = _context.Ratings.FirstOrDefault(
                r => r.CompanyId == ratingModel.CompanyId && r.UserId ==  userId);

            if (existingRate != null)
            {
                existingRate.Score = ratingModel.Score;
                _context.Ratings.Update(existingRate);
            } else
            {
                var rating = new Rating
                {
                    Score = ratingModel.Score,
                    CompanyId = ratingModel.CompanyId,
                    UserId = userId

                };
                _context.Ratings.Add(rating);
            }

            _context.SaveChanges();
            return RedirectToAction("CompanyDetail", new {id = ratingModel.CompanyId });
        }

        public IActionResult TripDetail(int id)
        {
            var currentUserId = _userManager.GetUserId(User);
            var Trip = _context.Trips.Find(id);
            var userRate = _context.Ratings.Where(r => r.UserId == currentUserId && r.TripId == id).Select(r => r.Score).FirstOrDefault();
            //if (Trip == null)
            //{
            //    return RedirectToAction("Error", "Home"); // Handle trip not found scenario
            //}

            var reviews = _context.Reviews.Include(r => r.User).Where(r => r.TripId == id)
        .Select(r => new
        {
            UserId = r.User.Id,
            UserName = r.User.FullName,
            UserImage = r.User.MainImage,
            UserComment = r.Comment,
            ReviewId = r.Id
        }).ToList();

            ViewData["Reviews"] = reviews;
            ViewData["currentReview"] = TempData["CurrentReview"];
            ViewData["CurrentUserId"] = currentUserId;
            ViewData["UserRate"] = userRate;
            return View(Trip);
        }



    }
}
