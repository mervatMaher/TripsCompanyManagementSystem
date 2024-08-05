using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TripsCompanySystem.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? MainImage { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripEndDate { get; set; }
        public string DestinationFrom {  get; set; }
        public string DestinationTo { get; set; }
        public int? Discount { get; set; }

        //[DataType(DataType.Currency)]
        public double? TripPrice {  get; set; }
        public double? TripPriceAfterDiscount { get; set;}
        public int? MaxTravels { get; set; }
        public int? ActualTravels { get; set; }
        public double? AverageRating => Ratings?.Any() == true ? Ratings.Average(r => r.Score) : 0.0;
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Favorite> Favorites { get; set;}
        public ICollection<Rating> Ratings { get; set; }


    }
}
