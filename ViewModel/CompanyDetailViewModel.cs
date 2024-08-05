using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class CompanyDetailViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public double NormalizedAverageRating { get; set; }
        public int UserRating { get; set; }
        public List<TripDetailViewModel> Trips { get; set; }
        public List<ApplicationUser> Moderatores { get; set; }
    }
}
