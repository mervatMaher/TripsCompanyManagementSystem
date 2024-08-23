using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Linq;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class TripController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;

        public TripController(TripsCompanySystemDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;    
        }
        public async Task<IActionResult> Favorites()
        {
            var userId =  _userManager.GetUserId(User);
            var userName = _context.Users.Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefault();

            var Favorites = _context.Favorites.Include(f => f.Trip).Where(f => f.UserId == userId)
                .Select(t => new
                {
                    UserId = t.UserId,
                    UserName = _context.Users.Where(u => u.Id == t.UserId).Select(u => u.FullName).FirstOrDefault(),
                    TripId = t.TripId,
                    TripName = _context.Trips.Where(tr => tr.Id == t.TripId).Select(t => t.Name).FirstOrDefault(),
                    TripDescription = _context.Trips.Where(tr => tr.Id == t.TripId).Select(t => t.Description).FirstOrDefault(),
                    TripImage = _context.Trips.Where(tr => tr.Id == t.TripId).Select(t => t.MainImage).FirstOrDefault()
                })
                .ToList();

            ViewBag.UserName = userName;
            ViewBag.Favorites = Favorites;
      
            return View();
        }

        [HttpPost]
        public IActionResult AddFavToList(int tripId)
        {
            var userId = _userManager.GetUserId(User);
            var existFavorite = _context.Favorites.FirstOrDefault(f => f.TripId == tripId && f.UserId == userId);

            var Fav = new Favorite
            {
                TripId = tripId,
                UserId = userId
            };

            if (existFavorite == null)
            {
                _context.Favorites.Add(Fav);
                _context.SaveChanges();
            }
            return RedirectToAction("Favorites");
        }
        public IActionResult Delete(int id)
        {
            var trip = _context.Favorites.Where(f => f.TripId == id).FirstOrDefault();
            if (trip != null)
            {
                _context.Favorites.Remove(trip);
                _context.SaveChanges();
            }

            return RedirectToAction("Favorites");
        }
        [HttpPost]
        public IActionResult Review (ReviewViewModel reviewModel)
        {
            var userId = _userManager.GetUserId(User);

            if (reviewModel != null)
            {
                Console.WriteLine($"Received TripId: {reviewModel.TripId}");
                bool tripExists = _context.Trips.Any(t => t.Id == reviewModel.TripId);

                var review = new Review
                {
                    Comment = reviewModel.Comment,
                    TripId = reviewModel.TripId,
                    UserId = userId
                };
                if (userId != null )
                {
                    _context.Reviews.Add(review);
                    _context.SaveChanges();
                } else
                {
                    return RedirectToAction("LogIn", "Account");
                }
            }

            return RedirectToAction("TripDetail", "Company", new { id = reviewModel.TripId });
        } 

        public IActionResult DeleteReview(int ReviewId)
        {
            var userId = _userManager.GetUserId(User);
            var reviewId = _context.Reviews.Where(r => r.Id == ReviewId).FirstOrDefault();
            var tripId = _context.Reviews.Where(r => r.Id == ReviewId)
                .Select(r => r.TripId).FirstOrDefault();
            if (reviewId != null)
            {
                _context.Reviews.Remove(reviewId);
                _context.SaveChanges();
            }
            return RedirectToAction("TripDetail", "Company", new { id = tripId });

        }

        public IActionResult TripRating(TripRatingViewModel ratingModel)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            var existingRate = _context.Ratings.FirstOrDefault(r => r.UserId == userId && r.TripId == ratingModel.TripId);

            if (existingRate != null)
            {
                existingRate.Score = ratingModel.Score;
                _context.Ratings.Update(existingRate);
            } else
            {
                var TripRate = new Rating
                {
                    Score = ratingModel.Score,
                    UserId = userId,
                    TripId = ratingModel.TripId
                };
                _context.Ratings.Add(TripRate);
            }
            _context.SaveChanges();

            return RedirectToAction("TripDetail", "Company", new {id = ratingModel.TripId});
        }


    }
}
