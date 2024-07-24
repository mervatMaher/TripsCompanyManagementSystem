using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class CompanyDetailModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public double NormalizedAverageRating { get; set; }
        public List<TripDetailModel> Trips { get; set; }
    }
}
