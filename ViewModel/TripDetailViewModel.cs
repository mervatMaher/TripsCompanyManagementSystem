using TripsCompanySystem.Models;

namespace TripsCompanySystem.ViewModel
{
    public class TripDetailViewModel
    {
        public int Id { get; set; }
        public string MainImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double NormalizedAverageRating { get; set; }
        public double? AverageRating {  get; set; }
        public int UserRating { get; set; }
       


    }
}
