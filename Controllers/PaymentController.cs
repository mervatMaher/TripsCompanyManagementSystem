using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stripe;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Stripe.TestHelpers;
using System.Linq;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TripsCompanySystem.Controllers
{
    public class PaymentController : Controller
    {

        private readonly TripsCompanySystemDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public PaymentController(TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Checkout(int bookingId)
        {
            ViewBag.StripPublishKey = _configuration["Stripe:PublishableKey"];

            var ViewModel = new CreatePaymentViewModel();
            var bookingAmount = _context.Bookings.Where(b => b.Id == bookingId).Select(b => b.TotalCost).FirstOrDefault();

            ViewBag.bookingId = bookingId;
            ViewBag.bookingAmount = bookingAmount;
            ViewBag.FormattedPaymentDate = ViewModel.PaymentDate.ToString("yyyy-MM-ddTHH:mm");
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(int bookingId, CreatePaymentViewModel paymentViewModel)
        {
            var booking = _context.Bookings.Find(bookingId);
            var userEmailWhoBook = _context.Bookings.Where(b => b.Id == bookingId).Select(u => u.Email).FirstOrDefault();
            var userId = _userManager.GetUserId(User);
            var userEmail = _context.Users.Where(u => u.Id == userId).Select(e => e.Email).FirstOrDefault();
            var userName = _context.Users.Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefault();
            try
            {
                ViewBag.StripPublishKey = _configuration["Stripe:PublishableKey"];

                var token = Request.Form["stripeToken"].ToString();

                var customerOptions = new CustomerCreateOptions
                {
                    Email = userEmailWhoBook,
                    Name = userName,
                    Source = token
                };

                var customerService = new Stripe.CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);

                var options = new ChargeCreateOptions
                {
                    Amount = (long)(paymentViewModel.Amount * 100), // Amount in cents
                    Currency = paymentViewModel.Currency.ToUpper(),
                    Description = "Payment for booking",
                    Customer = customer.Id,
                    Metadata = new Dictionary<string, string>
                    {
                        { "CustomerName", userName },
                        { "CustomerEmail", userEmailWhoBook }

                    },

                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if (ModelState.IsValid)
                {
                    var newPayment = new Payment
                    {
                        Amount = paymentViewModel.Amount,
                        StripePaymentId = charge.Id,
                        PaymentDate = paymentViewModel.PaymentDate,
                        Currency = paymentViewModel.Currency,
                        PaymentStatus = charge.Status,
                        BookingId = bookingId
                    };

                    bool paymentExists = _context.Payments.Any(p =>
                  p.StripePaymentId == newPayment.StripePaymentId);


                    if (!paymentExists)
                    {
                        _context.Payments.Add(newPayment);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Checkout", paymentViewModel);
                    }


                }
                else
                {

                    return RedirectToAction("Checkout", paymentViewModel);
                }
            }

            catch (StripeException ex)
            {
                ModelState.AddModelError("", "Payment failed: " + ex.Message);
                return View("Checkout", paymentViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return View("Checkout", paymentViewModel);
            }
        }

    }
}
