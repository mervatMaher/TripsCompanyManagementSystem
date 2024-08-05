using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class AddCompanyViewModel
    {
        public string CompanyName { get; set; }
        public IFormFile CompanyImage { get; set; }
        public string? ImageUrl { get; set; }

    }
}
