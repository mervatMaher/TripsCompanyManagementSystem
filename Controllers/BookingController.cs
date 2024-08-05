using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

namespace TripsCompanySystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public BookingController (TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public IActionResult BookingView(int tripId)
        {
            var trip = _context.Trips.Find(tripId);
            var bookingView = new BookingViewModel();
            ViewBag.Trip = trip;

            return View(bookingView);
        }

        public IActionResult SaveBooking( int tripId,BookingViewModel bookingView)
        {
            var userId = _userManager.GetUserId(User);
            var booking = new Booking
            {
                Name = bookingView.FullName,
                Email = bookingView.Email,
                PhoneNumber = bookingView.PhoneNumber,
                Address = bookingView.Address,
                NumOfFriends = bookingView.NumOfFriends,
                TotalCost = bookingView.TotalCost,
                TripId = tripId, 
                UserId = userId
                
            };

            if (booking != null )
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
            }
            return RedirectToAction("TripDetail", "Company", new {id = tripId });
        }
    }
}
