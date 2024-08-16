namespace TripsCompanySystem.ViewModel
{
    public class ModeratorCreateTripViewModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? MainImage { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripEndDate { get; set; }
        public string DestinationFrom { get; set; }
        public string DestinationTo { get; set; }
        public int? Discount { get; set; }
        public double? TripPrice { get; set; }
        public double? TripPriceAfterDiscount { get; set; }
        public string ImageUrl { get; set; }
    }
}
