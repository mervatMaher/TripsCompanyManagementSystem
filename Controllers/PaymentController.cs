using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using TripsCompanySystem.Data;
using TripsCompanySystem.Models;
using TripsCompanySystem.ViewModel;

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

        public PaymentController (TripsCompanySystemDbContext context, IServiceProvider serviceProvider, ILogger<AccountController> logger, IConfiguration configuration)
        {
            _context = context;
            _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration; 
        }
        public IActionResult CreatePayment([FromBody] CreatePaymentViewModel paymentViewModel )
        {

            return View();
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            return View();
        }
        [HttpGet("cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
