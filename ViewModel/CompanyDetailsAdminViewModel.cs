using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class CompanyDetailsAdminViewModel
    {
        public int CompanyId { get; set; }
        public string companyImage { get; set; }
        public string CompanyName { get; set; }
        public List<ApplicationUser> Moderatores { get; set; }
    }
}
